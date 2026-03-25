from datetime import date
from typing import List
from dataclasses import asdict

from logic.converter import dict_to_outage_scenario, dict_to_placement
from logic.kpi.kpi_models import KpiResult
from logic.planner import GreedyPlanner
from logic.outageReplanner import OutageReplanner
from logic.scoring import ConfigurableScoring
from logic.loaders import load_barns, load_flocks, load_paths, load_paths2, load_placement_lines
from logic.models import BarnOutageScenario, Paths, PlacementLine

# KPI imports
from logic.kpi.kpi_registry import evaluate_kpis
from logic.writers import save_kpi_results_to_csv, save_plan_to_csv

from pathlib import Path

BASE_DIR = Path(__file__).resolve().parent
DATA_DIR = BASE_DIR / "data"
DATA_DIR2 = "C:\\Users\\danis\\Documents\\FlockPlanner\\API_Data\\Data"
RESULTS_DIR = "C:\\Users\\danis\\Documents\\FlockPlanner\\API_Data\\Results"

CONFIG_DIR = "C:\\Users\\danis\\Documents\\FlockPlanner\\API_Data\\Config.json"

def load_data(paths: Paths):
    barns = load_barns(paths.barns_path)
    flocks = load_flocks(paths.flocks_path)
    scoring = ConfigurableScoring(paths.scoring_config_path)

    return barns, flocks, scoring

def load_data2():
    barns = load_barns(DATA_DIR / "barns.csv")
    flocks = load_flocks(DATA_DIR / "flocks.csv")

    scoring = ConfigurableScoring(DATA_DIR / "scoring_config.csv")

    return barns, flocks, scoring

def load_data3(guid):
    paths = load_paths2(CONFIG_DIR)
    barns = load_barns(f"{paths.InputFilePath}\\{guid}\\barns.csv")
    flocks = load_flocks(f"{paths.InputFilePath}\\{guid}\\flocks.csv")

    scoring = ConfigurableScoring(f"{paths.InputFilePath}\\{guid}\\scoring_config.csv")
    print(scoring)

    return barns, flocks, scoring, paths

def generate_plan():
    barns, flocks, scoring = load_data2()

    planner = GreedyPlanner(scoring)
    plan = planner.generate(flocks, barns)

    return plan

def generate_plan2(guid):
    barns, flocks, scoring, paths = load_data3(guid)

    planner = GreedyPlanner(scoring)
    plan = planner.generate(flocks, barns)

    save_plan_to_csv(plan, f"{paths.OutputFilePath}\\{guid}\\placement_lines.csv")

    return plan

def generate_plan_with_outage(paths: Paths, scenario: BarnOutageScenario):
    barns, flocks, scoring = load_data(paths)
    planner = GreedyPlanner(scoring)

    base_plan = planner.generate(flocks, barns)

    outage = OutageReplanner(planner)
    replanned = outage.replan(base_plan, flocks, barns, scenario)

    return replanned

def replan_for_outage2(paths: Paths, original_plan: List[PlacementLine], scenario: BarnOutageScenario):
    barns, flocks, scoring = load_data(paths)
    planner = GreedyPlanner(scoring)

    outage = OutageReplanner(planner)
    replanned = outage.replan(original_plan, flocks, barns, scenario)

    return replanned

def cached_replan_for_outage(guid: str, original_plan: List[PlacementLine], scenario: BarnOutageScenario) -> List[PlacementLine]:
    barns, flocks, scoring, paths = load_data3(guid)
    planner = GreedyPlanner(scoring)

    outage = OutageReplanner(planner)

    original_plan = [dict_to_placement(p) for p in original_plan]
    scenario = dict_to_outage_scenario(scenario)

    replanned = outage.replan(original_plan, flocks, barns, scenario)

    save_plan_to_csv(replanned, f"{paths.OutputFilePath}\\{guid}\\replanned_placement_lines.csv")

    return replanned

def replan_for_outage(guid: str, scenario: BarnOutageScenario) -> List[PlacementLine]:
    barns, flocks, scoring, paths = load_data3(guid)
    planner = GreedyPlanner(scoring)

    outage = OutageReplanner(planner)

    original_plan = load_placement_lines(f"{paths.OutputFilePath}\\{guid}", False)
    scenario = dict_to_outage_scenario(scenario)

    replanned = outage.replan(original_plan, flocks, barns, scenario)

    save_plan_to_csv(replanned, f"{paths.OutputFilePath}\\{guid}\\replanned_placement_lines.csv")

    return replanned

def generate_kpi_report(guid: str, placements2: List[PlacementLine], useReplanned: bool) -> List[KpiResult]:
    barns, flocks, scoring, paths = load_data3(guid)
    kpi_config_path = f"{paths.InputFilePath}\\{guid}\\kpis.csv"

    original_plan = load_placement_lines(f"{paths.OutputFilePath}\\{guid}", useReplanned=useReplanned)

    kpi_results = evaluate_kpis(
        kpi_config_path,
        placements=original_plan,
        barns=barns,
        flocks=flocks,
        scoring_config=scoring._values
    )

    save_kpi_results_to_csv(kpi_results, f"{paths.OutputFilePath}\\{guid}\\kpi_results.csv")

    return kpi_results