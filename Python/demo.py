from logic.planner import GreedyPlanner
from logic.outageReplanner import OutageReplanner
from logic.scoring import ConfigurableScoring
from logic.loaders import load_barns, load_flocks
from logic.models import BarnOutageScenario
from datetime import date

if __name__ == "__main__":
    barns = load_barns("data/barns.csv")
    flocks = load_flocks("data/flocks.csv")

    scoring = ConfigurableScoring("data/scoring_config.csv")
    planner = GreedyPlanner(scoring)

    base_plan = planner.generate(flocks, barns)

    print("=== INITIAL PLAN ===")
    for p in base_plan:
        print(p)

    outage = OutageReplanner(planner)
    scenario = BarnOutageScenario("B1", date(2025,3,1), date(2025,3,31))
    replanned = outage.replan(base_plan, flocks, barns, scenario)

    print("\n=== AFTER OUTAGE ===")
    for p in replanned:
        print(p)
