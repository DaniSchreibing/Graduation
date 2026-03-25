from dataclasses import dataclass
from typing import Optional

@dataclass
class KpiDefinition:
    name: str
    scope: str
    metric: str
    description: Optional[str] = None


@dataclass
class KpiResult:
    name: str
    scope: str
    value: float
    description: Optional[str] = None
