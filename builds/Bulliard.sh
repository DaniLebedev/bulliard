#!/bin/sh
printf '\033c\033]0;%s\a' Bulliard
base_path="$(dirname "$(realpath "$0")")"
"$base_path/Bulliard.x86_64" "$@"
