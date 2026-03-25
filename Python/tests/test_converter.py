from logic.converter import dict_to_placement, dict_to_outage_scenario
from logic.models import PlacementLine, BarnOutageScenario
from datetime import date

def test_dict_to_placement_full():
    data = {
        "flock_id": "F1",
        "barn_id": "B1",
        "planned_start": "2025-03-01",
        "allocated_qty": 300,
        "reason_unplaced": None,
    }

    obj = dict_to_placement(data)
    assert isinstance(obj, PlacementLine)
    assert obj.flock_id == "F1"
    assert obj.barn_id == "B1"
    assert obj.planned_start == date(2025, 3, 1)
    assert obj.allocated_qty == 300

def test_dict_to_placement_missing_date():
    data = {
        "flock_id": "F1",
        "barn_id": "B2",
        "planned_start": None,
        "allocated_qty": 200,
    }

    obj = dict_to_placement(data)
    assert obj.planned_start is None

def test_dict_to_outage_scenario():
    data = {
        "barn_id": "B5",
        "outage_start": date(2025, 4, 1),
        "outage_end": date(2025, 4, 10)
    }

    scenario = dict_to_outage_scenario(data)
    assert isinstance(scenario, BarnOutageScenario)
    assert scenario.barn_id == "B5"