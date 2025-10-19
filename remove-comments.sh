#!/bin/bash

# remove-comments.sh
# This script removes all comments from C# files in the solution
# Including: XML documentation (///), single-line (//), and multi-line (/* */)

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

# Default values
TARGET_DIR="${1:-.}"
WHATIF="${2:-false}"
FILES_PROCESSED=0
TOTAL_FILES=0

echo -e "${CYAN}Starting comment removal process...${NC}"
echo -e "${YELLOW}Target directory: $(realpath "$TARGET_DIR")${NC}"

if [ "$WHATIF" = "--whatif" ] || [ "$WHATIF" = "-w" ]; then
    echo -e "${MAGENTA}RUNNING IN WHATIF MODE - NO CHANGES WILL BE MADE${NC}"
    WHATIF="true"
else
    WHATIF="false"
fi
echo ""

# Find all .cs files (excluding obj, bin, node_modules)
mapfile -t CS_FILES < <(find "$TARGET_DIR" -type f -name "*.cs" \
    ! -path "*/obj/*" \
    ! -path "*/bin/*" \
    ! -path "*/node_modules/*")

TOTAL_FILES=${#CS_FILES[@]}
echo -e "${GREEN}Found $TOTAL_FILES C# files to process${NC}"
echo ""

# Process each file
for file in "${CS_FILES[@]}"; do
    echo -e "${GRAY}Processing: $(basename "$file")${NC}"

    # Create temporary file
    temp_file=$(mktemp)

    # Read and process file
    # Remove XML documentation comments (///)
    # Remove multi-line comments (/* */)
    # Remove single-line comments (//) but preserve URLs
    perl -0777 -pe '
        # Remove XML doc comments (/// ...)
        s/^\s*\/\/\/.*$//gm;

        # Remove multi-line comments (/* ... */)
        s/\/\*.*?\*\///gs;

        # Remove single-line comments (//) but keep http:// and https://
        s/(?<!:)\/\/(?!\/)[^\r\n]*//g;

        # Remove excessive empty lines (keep max 2 consecutive)
        s/(\n\s*){4,}/\n\n\n/g;
    ' "$file" > "$temp_file"

    # Check if file changed
    if ! cmp -s "$file" "$temp_file"; then
        if [ "$WHATIF" = "false" ]; then
            # Replace original file
            mv "$temp_file" "$file"
            echo -e "  ${GREEN}✓ Comments removed${NC}"
            ((FILES_PROCESSED++))
        else
            echo -e "  ${YELLOW}➜ Would remove comments (WhatIf mode)${NC}"
            ((FILES_PROCESSED++))
            rm "$temp_file"
        fi
    else
        echo -e "  ${GRAY}- No comments found${NC}"
        rm "$temp_file"
    fi
done

echo ""
echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}Summary:${NC}"
echo -e "  Total files scanned: ${TOTAL_FILES}"
echo -e "  ${GREEN}Files modified: ${FILES_PROCESSED}${NC}"
echo -e "  ${GRAY}Files unchanged: $((TOTAL_FILES - FILES_PROCESSED))${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

if [ "$WHATIF" = "true" ]; then
    echo -e "${MAGENTA}This was a dry run. To actually remove comments, run without --whatif${NC}"
else
    echo -e "${GREEN}Done! All comments have been removed.${NC}"
    echo -e "${YELLOW}Don't forget to commit your changes to git!${NC}"
fi
