import csv
from pathlib import Path
from .models import Barn, FlockBatch


class ConfigurableScoring:
    def __init__(self, config_path: str):
        self._values = {}
        with open(config_path, newline="") as f:
            reader = csv.DictReader(f)
            for row in reader:
                self._values[row["key"]] = float(row["value"])

        self.capacity_weight = self._values["Capacity Weight"]
        self.region_penalty = self._values["Region Penalty"]
        self.housing_penalty = self._values["Housing Penalty"]
        self.region_multiplier = self._values["Region Multiplier"]
        self.housing_multiplier = self._values["Housing Multiplier"]
    def score(self, barn: Barn, flock: FlockBatch, remaining_capacity: int) -> float:
        capacity_penalty = abs(remaining_capacity - flock.size) * self.capacity_weight
        region_penalty = 0 if barn.region == flock.region else self.region_penalty
        housing_penalty = 0 if barn.housing_type == flock.housing_type else self.housing_penalty

        return (
            capacity_penalty +
            region_penalty * self.region_multiplier +
            housing_penalty * self.housing_multiplier
        )
