from typing import Dict, List, Optional
from ..models import PlacementLine, Barn, FlockBatch
from .kpi_loader import load_kpi_registry
from .kpi_engine import KpiEngine
from .kpi_models import KpiResult


def evaluate_kpis(
    kpi_csv_path: str,
    placements: List[PlacementLine],
    barns: List[Barn],
    flocks: List[FlockBatch],
    scoring_config: Optional[Dict[str, float]] = None
) -> List[KpiResult]:

    registry = load_kpi_registry(kpi_csv_path)
    engine = KpiEngine(registry)

    return engine.evaluate(placements, barns, flocks, scoring_config=scoring_config)
