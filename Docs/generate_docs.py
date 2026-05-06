#!/usr/bin/env python3
"""Generates a single-file HTML API reference from the SwiftDrop XML documentation."""

import xml.etree.ElementTree as ET
import html
import re
import sys
from pathlib import Path
from collections import defaultdict

XML_PATH = Path(__file__).parent.parent / "SwiftDrop/bin/Release/net9.0/SwiftDrop.xml"
OUT_PATH  = Path(__file__).parent / "api_reference.html"

def clean(text: str) -> str:
    if not text:
        return ""
    text = re.sub(r"\s+", " ", text)
    text = re.sub(r'<see cref="[A-Z]:([^"]+)"', r"<code>\1</code>", text)
    text = re.sub(r'<c>([^<]*)</c>', r"<code>\1</code>", text)
    text = re.sub(r'<paramref name="([^"]+)"\s*/>', r"<code>\1</code>", text)
    return text.strip()

def get_text(elem) -> str:
    if elem is None:
        return ""
    parts = []
    if elem.text:
        parts.append(elem.text)
    for child in elem:
        parts.append(ET.tostring(child, encoding="unicode"))
        if child.tail:
            parts.append(child.tail)
    return clean("".join(parts))

def short_name(full: str) -> str:
    return full.split(".")[-1]

def namespace_of(full: str) -> str:
    parts = full.split(".")
    return ".".join(parts[:-1]) if len(parts) > 1 else ""

def parse_members(xml_path: Path):
    tree = ET.parse(xml_path)
    root = tree.getroot()
    members = defaultdict(dict)

    for member in root.findall(".//member"):
        name_attr = member.get("name", "")
        if not name_attr:
            continue

        kind = name_attr[0]
        full = name_attr[2:]

        summary = get_text(member.find("summary"))
        returns  = get_text(member.find("returns"))
        remarks  = get_text(member.find("remarks"))
        params   = {p.get("name"): get_text(p) for p in member.findall("param")}
        exceptions = {e.get("cref", ""): get_text(e) for e in member.findall("exception")}

        members[kind][full] = {
            "summary": summary,
            "returns": returns,
            "remarks": remarks,
            "params": params,
            "exceptions": exceptions,
        }

    return members

def group_by_namespace(type_members: dict) -> dict:
    groups = defaultdict(list)
    for full_name in sorted(type_members.keys()):
        ns = namespace_of(full_name)
        groups[ns].append(full_name)
    return groups

def render_member_block(full: str, info: dict, kind: str) -> str:
    label = {"M": "method", "P": "property", "F": "field", "E": "event"}.get(kind, "member")
    name = short_name(full.split("(")[0])
    sig  = full.split("(")[1].rstrip(")") if "(" in full else ""

    parts = [f'<div class="member">']
    parts.append(f'<h4><span class="badge {label}">{label}</span> <code>{html.escape(name)}</code></h4>')
    if info["summary"]:
        parts.append(f'<p>{info["summary"]}</p>')
    if info["params"]:
        parts.append('<table class="params"><tr><th>Parameter</th><th>Description</th></tr>')
        for pname, pdesc in info["params"].items():
            parts.append(f'<tr><td><code>{html.escape(pname)}</code></td><td>{pdesc}</td></tr>')
        parts.append('</table>')
    if info["returns"]:
        parts.append(f'<p><strong>Returns:</strong> {info["returns"]}</p>')
    if info["exceptions"]:
        for exc, desc in info["exceptions"].items():
            exc_short = exc.split(":")[-1]
            parts.append(f'<p><strong>Throws</strong> <code>{html.escape(exc_short)}</code>: {desc}</p>')
    if info["remarks"]:
        parts.append(f'<p class="remarks"><em>Remarks:</em> {info["remarks"]}</p>')
    parts.append('</div>')
    return "\n".join(parts)

def render_type(full_type: str, type_info: dict, all_members: dict) -> str:
    name = short_name(full_type)
    parts = [f'<section id="{html.escape(full_type)}" class="type-section">']
    parts.append(f'<h3>{html.escape(name)}</h3>')
    if type_info["summary"]:
        parts.append(f'<p class="type-summary">{type_info["summary"]}</p>')

    for kind, kind_dict in all_members.items():
        if kind == "T":
            continue
        matching = {k: v for k, v in kind_dict.items()
                    if k.startswith(full_type + ".") or k.startswith(full_type + "(")
                    or (k.startswith(full_type + "#") )}
        for full_m, info in sorted(matching.items()):
            parts.append(render_member_block(full_m, info, kind))

    parts.append('</section>')
    return "\n".join(parts)

CSS = """
* { box-sizing: border-box; margin: 0; padding: 0; }
body { font-family: 'Segoe UI', sans-serif; font-size: 14px; color: #1a1a2e; background: #f8f9fa; }
header { background: #16213e; color: white; padding: 20px 40px; }
header h1 { font-size: 1.8rem; }
header p { opacity: .8; margin-top: 4px; }
nav { background: #0f3460; width: 260px; position: fixed; top: 0; left: 0; height: 100vh;
      overflow-y: auto; padding-top: 80px; }
nav a { display: block; color: #cdd3e0; padding: 6px 20px; text-decoration: none; font-size: 13px; }
nav a:hover { background: rgba(255,255,255,.1); color: white; }
nav .ns-header { color: #a0aec0; font-size: 11px; padding: 12px 20px 4px;
                 text-transform: uppercase; letter-spacing: 1px; }
main { margin-left: 260px; padding: 40px; max-width: 980px; }
.type-section { background: white; border-radius: 8px; padding: 28px;
                margin-bottom: 28px; box-shadow: 0 1px 4px rgba(0,0,0,.08); }
.type-section h3 { font-size: 1.3rem; color: #16213e; border-bottom: 2px solid #e2e8f0;
                   padding-bottom: 8px; margin-bottom: 14px; }
.type-summary { color: #4a5568; margin-bottom: 16px; line-height: 1.6; }
.member { border-left: 3px solid #e2e8f0; padding: 10px 16px; margin: 10px 0; }
.member h4 { font-size: .95rem; margin-bottom: 6px; }
.member p { color: #4a5568; margin: 4px 0; line-height: 1.5; }
.remarks { color: #718096; font-style: italic; }
.badge { display: inline-block; padding: 1px 7px; border-radius: 3px; font-size: 11px;
         font-weight: 600; text-transform: uppercase; margin-right: 6px; }
.badge.method   { background: #ebf8ff; color: #2b6cb0; }
.badge.property { background: #f0fff4; color: #276749; }
.badge.field    { background: #fff5f5; color: #c53030; }
.badge.event    { background: #faf5ff; color: #6b46c1; }
.badge.member   { background: #fffaf0; color: #c05621; }
table.params { border-collapse: collapse; width: 100%; margin: 8px 0; font-size: 13px; }
table.params th { background: #f7fafc; text-align: left; padding: 5px 10px;
                  border-bottom: 1px solid #e2e8f0; }
table.params td { padding: 4px 10px; border-bottom: 1px solid #f0f0f0; vertical-align: top; }
code { background: #edf2f7; padding: 1px 5px; border-radius: 3px; font-family: monospace; font-size: 12px; }
"""

def generate(xml_path: Path, out_path: Path):
    members = parse_members(xml_path)
    types   = members.get("T", {})
    groups  = group_by_namespace(types)

    nav_html = []
    body_html = []

    for ns in sorted(groups.keys()):
        nav_html.append(f'<div class="ns-header">{html.escape(ns)}</div>')
        for full_type in groups[ns]:
            name = short_name(full_type)
            nav_html.append(f'<a href="#{html.escape(full_type)}">{html.escape(name)}</a>')
            body_html.append(render_type(full_type, types[full_type], members))

    page = f"""<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>SwiftDrop — API Reference</title>
<style>{CSS}</style>
</head>
<body>
<nav>
<div style="padding:16px 20px;color:white;font-weight:700;font-size:1rem;background:#0d2137;">
  SwiftDrop API
</div>
{"".join(nav_html)}
</nav>
<main>
<header style="margin:-40px -40px 32px;padding:28px 40px;background:#16213e;color:white;margin-left:-40px;">
  <h1>SwiftDrop — API Reference</h1>
  <p>Auto-generated from XML documentation comments. SwiftDrop v1.0 · ASP.NET Core 9</p>
</header>
{"".join(body_html)}
</main>
</body>
</html>"""

    out_path.write_text(page, encoding="utf-8")
    print(f"Documentation written to: {out_path}")
    print(f"Types documented: {len(types)}")

if __name__ == "__main__":
    if not XML_PATH.exists():
        print(f"ERROR: XML not found at {XML_PATH}")
        print("Run: dotnet build SwiftDrop/SwiftDrop.csproj -c Release")
        sys.exit(1)
    generate(XML_PATH, OUT_PATH)
