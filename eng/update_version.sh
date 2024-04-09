#!/bin/bash

# Read the current version from the file
versionFile="./eng/Versions.props"
currentVersion=$(grep -oP "<VersionPrefix>\K(.*)(?=</VersionPrefix>)" "$versionFile" | sed 's/\s*//')

# Parse the current version
IFS='.' read -r major minor patch <<< "$currentVersion"

# Increment the patch version
newPatch=$((patch + 1))
newVersion="$major.$minor.$newPatch"

# Update the version
sed -i "s|<VersionPrefix>[^<]*</VersionPrefix>|<VersionPrefix>$newVersion</VersionPrefix>|" "$versionFile"
echo "The VersionPrefix in $versionFile has been bumped up from $currentVersion to $newVersion"

# Add the updated version file to the staging area
git add "$versionFile"
