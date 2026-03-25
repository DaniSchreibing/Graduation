from typing import List
from .models import PlacementLine, FlockBatch, Barn, BarnOutageScenario
from .planner import GreedyPlanner

class OutageReplanner:
    def __init__(self, planner: GreedyPlanner):
        self.planner = planner

    def replan(
        self,
        base_plan: List[PlacementLine],
        flocks: List[FlockBatch],
        barns: List[Barn],
        scenario: BarnOutageScenario
    ) -> List[PlacementLine]:

        unaffected: List[PlacementLine] = []
        displaced_ids = set()
        pre_allocated = {}

        for line in base_plan:
            if line.barn_id == scenario.barn_id:
                displaced_ids.add(line.flock_id)
            else:
                unaffected.append(line)
                if line.barn_id:
                    pre_allocated[line.barn_id] = pre_allocated.get(line.barn_id, 0) + line.allocated_qty

        displaced_flocks = [f for f in flocks if f.id in displaced_ids]
        remaining_barns = [b for b in barns if b.id != scenario.barn_id]

        replanned = self.planner.generate(displaced_flocks, remaining_barns, pre_allocated)
        
        return unaffected + replanned
