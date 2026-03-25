from logic.planner import GreedyPlanner
from logic.scoring import ConfigurableScoring
from logic.models import Barn, FlockBatch
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

def test_greedy_planner_assigns_to_best_barn(tmp_path):
    scoring = make_scoring(tmp_path)
    planner = GreedyPlanner(scoring)

    barns = [
        Barn("B1", 1000, "CAGE", "NL"),
        Barn("B2", 600, "CAGE", "DE"),
    ]

    flocks = [
        FlockBatch("F1", 500, "CAGE", date(2025, 3, 1), date(2025, 3, 10), "NL"),
    ]

    plan = planner.generate(flocks, barns)

    assert len(plan) == 1

    # Expected: B2 (smaller capacity → lower capacity penalty)
    assert plan[0].barn_id == "B2"

def test_greedy_planner_handles_insufficient_capacity(tmp_path):
    scoring = make_scoring(tmp_path)
    planner = GreedyPlanner(scoring)

    barns = [Barn("B1", 300, "CAGE", "NL")]
    flocks = [
        FlockBatch("F1", 500, "CAGE", date(2025, 3, 1), date(2025, 3, 10), "NL")
    ]

    plan = planner.generate(flocks, barns)

    assert plan[0].barn_id == '"No barn found"'
    assert plan[0].reason_unplaced == "No suitable barn capacity"


def test_planner_preallocated_reduces_capacity(tmp_path):
    scoring = make_scoring(tmp_path)
    planner = GreedyPlanner(scoring)

    barns = [Barn("B1", 1000, "CAGE", "NL")]
    pre_alloc = {"B1": 900}

    flocks = [
        FlockBatch("F1", 200, "CAGE", date(2025, 4, 1), date(2025, 4, 10), "NL")
    ]

    plan = planner.generate(flocks, barns, pre_alloc)

    # Only 100 capacity left, but flock needs 200
    assert plan[0].barn_id == '"No barn found"'
    assert plan[0].reason_unplaced == "No suitable barn capacity"

def test_planner_tie_breaking_prefers_smaller_capacity(tmp_path):
    scoring = make_scoring(tmp_path)
    planner = GreedyPlanner(scoring)

    barns = [
        Barn("B1", 1000, "CAGE", "NL"),
        Barn("B2", 900, "CAGE", "NL"),
    ]

    flocks = [
        FlockBatch("F1", 100, "CAGE", date(2025, 4, 1), date(2025, 4, 10), "NL")
    ]

    plan = planner.generate(flocks, barns)

    # B2 gives lower capacity penalty
    assert plan[0].barn_id == "B2"
