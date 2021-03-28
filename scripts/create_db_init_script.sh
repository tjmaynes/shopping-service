#!/bin/bash

set -e

check_requirements() {
    if [[ -z "$PROJECT_NAME" ]]; then
        echo "Please provide a name for the project."
        exit 1
    elif [[ -z "$DB_NAME" ]]; then
        echo "Please provide a name for the database."
        exit 1
    elif [[ -z "$DB_USER" ]]; then
        echo "Please provide a user for accessing the database."
        exit 1
    fi
}

main() {
    check_requirements

    mkdir -p /tmp/$PROJECT_NAME || true
    (rm -f /tmp/$PROJECT_NAME/init.sql || true) && cat <<EOF | tee -a /tmp/$PROJECT_NAME/init.sql >/dev/null
GRANT ALL PRIVILEGES ON DATABASE "${DB_NAME}" to ${DB_USER};
EOF
}

main