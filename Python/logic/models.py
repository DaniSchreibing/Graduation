from dataclasses import dataclass
from datetime import date
from typing import List, Optional, Dict

@dataclass
class Barn:
    id: str
    capacity: int
    housing_type: str
    region: str

@dataclass
class FlockBatch:
    id: str
    size: int
    housing_type: str
    target_start: date
    target_end: date
    region: str

@dataclass
class PlacementLine:
    flock_id: str
    barn_id: Optional[str]
    planned_start: Optional[date]
    allocated_qty: int
    reason_unplaced: Optional[str] = None

@dataclass
class BarnOutageScenario:
    barn_id: str
    outage_start: date
    outage_end: date

@dataclass
class Paths:
    barns_path: str
    flocks_path: str
    scoring_config_path: str
    kpi_config_path: str

@dataclass
class FilePaths:
    InputFilePath: str
    OutputFilePath: str
    StatusFilePath: str
    LogFilePath: str
    PythonCodePath: str
    PythonDLL: str