from logic.planner import GreedyPlanner
from logic.outageReplanner import OutageReplanner
from logic.scoring import ConfigurableScoring
from logic.models import Barn, FlockBatch, BarnOutageScenario, PlacementLine
from datetime import date

def make_scoring(tmp_path):
    config_file = tmp_path / "scoring.csv"
    config_file.write_text(
        "key,value\n"
        "Capacity Weight,1\n"
        "Region Penalty,50\n"
        "Housing Penalty,1000\n"
        "Region Multiplier,2\n"
        "Housing Multiplier,10\n"
    )
    return ConfigurableScoring(str(config_file))

def test_outage_replans_displaced_flocks(tmp_path):
    scoring = make_scoring(tmp_path)
    planner = GreedyPlanner(scoring)
    outage = OutageReplanner(planner)

    barns = [
        Barn("B1", 1000, "CAGE", "NL"),
        Barn("B2", 1200, "CAGE", "NL"),
    ]
    flocks = [
        FlockBatch("F1", 400, "CAGE", date(2025, 3, 1), date(2025, 3, 10), "NL"),
        FlockBatch("F2", 350, "CAGE", date(2025, 3, 2), date(2025, 3, 12), "NL"),
    ]

    base_plan = planner.generate(flocks, barns)
    scenario = BarnOutageScenario("B1", date(2025, 3, 1), date(2025, 3, 31))

    replanned = outage.replan(base_plan, flocks, barns, scenario)

    for line in replanned:
        if line.flock_id in ("F1", "F2"):
            assert line.barn_id == "B2"


def test_outage_no_displaced(tmp_path):
    scoring = make_scoring(tmp_path)
    planner = GreedyPlanner(scoring)
    outage = OutageReplanner(planner)

    barns = [Barn("B1", 1000, "CAGE", "NL")]
    flocks = [
        FlockBatch("F1", 200, "CAGE", date(2025, 1, 1), date(2025, 1, 5), "NL")
    ]

    base_plan = [
        PlacementLine("F1", "B1", date(2025, 1, 1), 200)
    ]

    scenario = BarnOutageScenario("B2", date(2025, 2, 1), date(2025, 2, 28))

    # No barns closed → plan unchanged
    new_plan = outage.replan(base_plan, flocks, barns, scenario)
    assert len(new_plan) == 1
    assert new_plan[0].barn_id == "B1"
