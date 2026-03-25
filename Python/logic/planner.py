from typing import List, Dict, Optional
from .models import Barn, FlockBatch, PlacementLine

class GreedyPlanner:
    def __init__(self, scoring_strategy):
        self.scoring = scoring_strategy

    def generate(
        self,
        flocks: List[FlockBatch],
        barns: List[Barn],
        pre_allocated: Optional[Dict[str, int]] = None
    ) -> List[PlacementLine]:

        remaining_capacity = {b.id: b.capacity for b in barns}

        if pre_allocated:
            for barn_id, qty in pre_allocated.items():
                if barn_id in remaining_capacity:
                    remaining_capacity[barn_id] -= qty

        placement_lines: List[PlacementLine] = []
        flocks_sorted = sorted(flocks, key=lambda f: (f.target_start, -f.size))

        for flock in flocks_sorted:
            best_barn = None
            best_score = float("inf")

            for barn in barns:
                if remaining_capacity[barn.id] < flock.size:
                    continue

                score = self.scoring.score(barn, flock, remaining_capacity[barn.id])

                if score < best_score:
                    best_score = score
                    best_barn = barn

            if best_barn:
                remaining_capacity[best_barn.id] -= flock.size
                placement_lines.append(
                    PlacementLine(
                        flock_id=flock.id,
                        barn_id=best_barn.id,
                        planned_start=flock.target_start,
                        allocated_qty=flock.size
                    )
                )
            else:
                placement_lines.append(
                    PlacementLine(
                        flock_id=flock.id,
                        barn_id='"No barn found"',
                        planned_start=flock.target_start,
                        allocated_qty=0,
                        reason_unplaced="No suitable barn capacity"
                    )
                )

        return placement_lines
