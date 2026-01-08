# File: create_movidb_archive.py
# Usage: python create_movidb_archive.py
# Output: MoviDBPackage.zip in the current directory

import os
import shutil
from zipfile import ZipFile, ZIP_DEFLATED

# Define archive name
archive_name = "MoviDBPackage.zip"

# Directories and files to include
items_to_include = {
    "doc": "doc",
    "MoviDB/bin/Release/net10.0/win-x64": "bin",
    "sql": "sql",
    "samples": "samples",
    "README.md": "README.md",
    "DatabaseConfig.json":"DatabaseConfig.json"
}

# Remove existing archive if it exists
if os.path.exists(archive_name):
    os.remove(archive_name)

# Create zip archive
with ZipFile(archive_name, 'w', ZIP_DEFLATED) as zipf:
    for src_path, arc_name in items_to_include.items():
        if not os.path.exists(src_path):
            print(f"Warning: {src_path} does not exist, skipping.")
            continue

        if os.path.isfile(src_path):
            zipf.write(src_path, arc_name)
        else:
            for root, dirs, files in os.walk(src_path):
                for file in files:
                    full_path = os.path.join(root, file)
                    # Compute archive path
                    rel_path = os.path.relpath(full_path, src_path)
                    zipf.write(full_path, os.path.join(arc_name, rel_path))

print(f"Archive created successfully: {archive_name}")
