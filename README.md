# My Extendable Hack (meh)

This is an extendable utility app to help automate workflows.

## Commands

### `clean-path`

Removes old files, and any folders left empty, from a given path. The path
itself is never removed.

```shell
meh clean-path <path> [--age <days>] [--source <source>] [--dry-run]
```

Alias: `clean-dir`

| Argument / Option  | Description                                                 | Default            |
| ------------------ | ----------------------------------------------------------- | ------------------ |
| `<path>`           | The directory to clean. It must exist.                      | (required)         |
| `-a`, `--age`      | The maximum age of a file in days. Must be greater than 0.  | `15`               |
| `-s`, `--source`   | Where a file's age is obtained from: `LastModifiedTime`.    | `LastModifiedTime` |
| `-n`, `--dry-run`  | Lists what would be deleted without deleting anything.      | off                |

- A file is removed once its age is greater than or equal to `--age` days.
- Subdirectories are processed depth first, so a folder emptied by the clean
  is removed too.
- Linked directories (symbolic links and junctions) are skipped and never
  followed.
- A file or folder that can't be read or deleted is reported and skipped; the
  command exits with `1` if any errors occurred.

```shell
# Preview what would be removed from ~/Downloads after 30 days
meh clean-path ~/Downloads --age 30 --dry-run
```

### `git fix-mv`

Corrects a file move performed outside of Git tooling (e.g. in an IDE or file
manager) so Git records it as a rename and keeps the file's history, while
preserving any changes made to the moved file.

```shell
meh git fix-mv [<origin-path>] [<target-path>] [--repository-root <path>]
```

Alias: `fix-move`

| Argument / Option                         | Description                                      |
| ----------------------------------------- | ------------------------------------------------ |
| `<origin-path>`                           | Where the file originated (the deleted file).    |
| `<target-path>`                           | Where the file resides now (the untracked file). |
| `-r`, `--repos-root`, `--repository-root` | The repository's root path.                      |

- When `<origin-path>` is omitted, you're prompted to pick from the tracked
  files that have been deleted.
- When `<target-path>` is omitted, you're prompted to pick from the untracked
  files.
- When `--repository-root` is omitted, the root is found by searching up from
  the given file (or the current directory). Git worktrees are supported.

How it works: the file at the target is temporarily renamed, the original is
restored and moved with `git mv`, and then the target's contents are put back
in place.

```shell
# Pick the origin and target files interactively
meh git fix-mv
```
