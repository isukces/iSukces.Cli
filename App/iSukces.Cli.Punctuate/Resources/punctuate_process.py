import sys
import json
from transformers import pipeline

IN_FILE = sys.argv[1]
OUT_FILE = sys.argv[2]
MODEL = sys.argv[3] if len(sys.argv) > 3 else "kredor/punctuate-all"

def json_fix(x):
    if hasattr(x, "item"):
        return x.item()
    raise TypeError(f"Not JSON serializable: {type(x)}")

text = open(IN_FILE, "r", encoding="utf-8").read().strip()

pipe = pipeline(
    "token-classification",
    model=MODEL,
    aggregation_strategy="none"
)

result = pipe(text)

with open(OUT_FILE, "w", encoding="utf-8") as f:
    json.dump(result, f, ensure_ascii=False, default=json_fix)