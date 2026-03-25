from datetime import date
from typing import List

from logic.models import BarnOutageScenario, PlacementLine

def dict_to_placement(d: dict) -> PlacementLine:
    planned_start = (
        date.fromisoformat(d["planned_start"])
        if d.get("planned_start")
        else None
    )

    return PlacementLine(
        flock_id=d["flock_id"],
        barn_id=d.get("barn_id"),
        planned_start=planned_start,
        allocated_qty=d["allocated_qty"],
        reason_unplaced=d.get("reason_unplaced"),
    )

def dict_to_outage_scenario(d: dict) -> BarnOutageScenario:
    return BarnOutageScenario(
        barn_id=d["barn_id"],
        outage_start=d["outage_start"],
        outage_end=d["outage_end"],
    )
