from typing import List
import csv
from datetime import date
from importlib.resources import path
from pathlib import Path
from .models import Barn, FilePaths, FlockBatch, Paths, PlacementLine
import json

def load_barns(path: str):
    barns = []
    with open(path, newline="") as f:
        reader = csv.DictReader(f)
        for row in reader:
            barns.append(
                Barn(
                    id=row["Name"],
                    capacity=int(row["Capacity"]),
                    housing_type=row["Housing Type"],
                    region=row["Region"]
                )
            )
    return barns

def load_flocks(path: str):
    flocks = []
    with open(path, newline="") as f:
        reader = csv.DictReader(f)
        for row in reader:
            flocks.append(
                FlockBatch(
                    id=row["id"],
                    size=int(row["size"]),
                    housing_type=row["housing_type"],
                    target_start=date.fromisoformat(row["target_start"]),
                    target_end=date.fromisoformat(row["target_end"]),
                    region=row["region"]
                )
            )
    return flocks

def load_paths(path: str):
    with open(path) as f:
        return json.load(f)
    
def load_paths2(config_file: str | Path) -> FilePaths:
    with open("C:\\Users\\danis\\Documents\\FlockPlanner\\API_Data\\Config.json", "r", encoding="utf-8") as f:
        data = json.load(f)

    return FilePaths(**data)

def load_placement_lines(path: str, useReplanned: bool) -> List[PlacementLine]:
    placement_lines = []
    file_name = "replanned_placement_lines.csv" if useReplanned else "placement_lines.csv"
    
    with open(f"{path}\\{file_name}", newline="") as f:
        reader = csv.DictReader(f)
        for row in reader:
            placement_lines.append(
                PlacementLine(
                    flock_id=row["flock_id"],
                    barn_id=row["barn_id"],
                    planned_start=date.fromisoformat(row["planned_start"]),
                    allocated_qty=int(row["allocated_qty"]),
                    reason_unplaced=row["reason_unplaced"]
                )
            )
    return placement_lines