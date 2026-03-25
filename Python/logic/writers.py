import csv
from dataclasses import asdict
from pathlib import Path
from datetime import date
from typing import List

from logic.models import PlacementLine
from logic.kpi.kpi_models import KpiResult

def save_plan_to_csv(plan: List[PlacementLine], file_path: str) -> None:
    path = Path(file_path)
    path.parent.mkdir(parents=True, exist_ok=True)

    with path.open("w", newline="", encoding="utf-8") as f:
        writer = csv.DictWriter(
            f,
            fieldnames=PlacementLine.__dataclass_fields__.keys(),
        )
        writer.writeheader()

        for line in plan:
            row = asdict(line)

            # Convert date objects to ISO strings for CSV
            if isinstance(row["planned_start"], date):
                row["planned_start"] = row["planned_start"].isoformat()

            writer.writerow(row)

    # Keep CSV defaults for all fields, but normalize the specific unplaced barn marker.
    content = path.read_text(encoding="utf-8")
    content = content.replace('"""No barn found"""', '"No barn found"')
    path.write_text(content, encoding="utf-8")


def save_kpi_results_to_csv(kpi_results: List[KpiResult], file_path: str) -> None:
    path = Path(file_path)
    path.parent.mkdir(parents=True, exist_ok=True)

    with path.open("w", newline="", encoding="utf-8") as f:
        writer = csv.DictWriter(
            f,
            fieldnames=KpiResult.__dataclass_fields__.keys()
        )
        writer.writeheader()

        for result in kpi_results:
            row = asdict(result)
            writer.writerow(row)