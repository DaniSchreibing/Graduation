from logic.planner import GreedyPlanner
from logic.outageReplanner import OutageReplanner
from logic.scoring import ConfigurableScoring
from logic.loaders import load_barns, load_flocks
from logic.models import BarnOutageScenario
from datetime import date
import argparse

def load_data():
    barns = load_barns("data/barns.csv")
    flocks = load_flocks("data/flocks.csv")
    return barns, flocks

def generate_plan():
    barns, flocks = load_data()
    scoring = ConfigurableScoring("data/scoring_config.csv")
    planner = GreedyPlanner(scoring)

    plan = planner.generate(flocks, barns)

    print("=== INITIAL PLAN ===")
    for p in plan:
        print(p)

    return plan

def replan_for_outage():
    barns, flocks = load_data()
    scoring = ConfigurableScoring("data/scoring_config.csv")
    planner = GreedyPlanner(scoring)

    base_plan = planner.generate(flocks, barns)

    outage = OutageReplanner(planner)
    scenario = BarnOutageScenario("B1", date(2025, 3, 1), date(2025, 3, 31))
    replanned = outage.replan(base_plan, flocks, barns, scenario)

    print("=== AFTER OUTAGE ===")
    for p in replanned:
        print(p)

def generate_kpi_report():
    from logic.kpi.kpi_registry import evaluate_kpis

    barns, flocks = load_data()
    scoring = ConfigurableScoring("data/scoring_config.csv")
    plan = generate_plan()

    print("=== KPI RESULTS ===")
    kpi_results = evaluate_kpis(
        "data/kpis.csv",
        placements=plan,
        barns=barns,
        flocks=flocks,
        scoring_config=scoring._values
    )

    for k in kpi_results:
        print(f"{k.name}: {k.value}  ({k.description})")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Barn planning CLI")

    parser.add_argument(
        "command",
        choices=["load", "plan", "replan", "kpi"],
        help="Which operation to run"
    )

    args = parser.parse_args()

    if args.command == "load":
        load_data()
    elif args.command == "plan":
        generate_plan()
    elif args.command == "replan":
        replan_for_outage()
    elif args.command == "kpi":
        generate_kpi_report()
