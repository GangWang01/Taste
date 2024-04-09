#!/bin/bash

# Function to get the current git branch name
get_git_branch() {
    git rev-parse --abbrev-ref HEAD
}

# Call the function and store the result
branch_name=$(get_git_branch)
if [[ "$branch_name" =~ "^rel/[0-9]+\.[0-9]+$" ]]; then

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

fi

