import csv
from pathlib import Path
from typing import List
from .kpi_models import KpiDefinition


def load_kpi_registry(path: str) -> List[KpiDefinition]:
    registry = []

    with open(path, newline="") as f:
        reader = csv.DictReader(f)

        for row in reader:
            registry.append(
                KpiDefinition(
                    name=row["kpi_name"],
                    scope=row["scope"],
                    metric=row["metric"],
                    description=row.get("description")
                )
            )

    return registry
