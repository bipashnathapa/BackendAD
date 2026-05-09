#!/usr/bin/env bash
# ===============================================================
# 05-git-push.sh
# Pushes new feature work to a fresh branch on the existing remote
# (you are a collaborator, repo owned by someone else).
#
# What this does:
#   1. Verifies git remote + your branch state
#   2. Creates feature branch (if not on one already)
#   3. Adds .gitignore entries for build output, secrets, junk
#   4. Removes accidental backup files from the worktree
#   5. Stages all relevant project files (NOT junk/secrets/backups)
#   6. Commits with a descriptive message
#   7. Pushes the branch and sets upstream
# ===============================================================
set -euo pipefail

ROOT="$(pwd)"
[ -f "$ROOT/VehicleManagement.sln" ] || { echo "❌ Run from BackendAD project root"; exit 1; }
[ -d "$ROOT/.git" ] || { echo "❌ Not a git repo. Did you 'git clone' originally?"; exit 1; }

BRANCH="${1:-feature/admin-vendor-parts-sales-reports}"

echo "==> [1/8] Git status check"
git remote -v
echo ""
echo "Current branch: $(git rev-parse --abbrev-ref HEAD)"
echo ""

# Make sure we're synced with the remote main first to avoid conflicts later
echo "==> [2/8] Fetching latest from remote"
git fetch origin

echo ""
echo "==> [3/8] Cleaning junk files from worktree"
# These are build/runtime byproducts that should never be committed
JUNK_FILES=(
  "VehicleManagement.API/appsettings.json.bak.1777964294"
  "dotnet-install.sh"
  "postgresql.conf"
  "1.txt"
)
for f in "${JUNK_FILES[@]}"; do
  if [ -e "$ROOT/$f" ]; then
    rm -f "$ROOT/$f"
    echo "    removed: $f"
  fi
done

# Find any other appsettings backup files
find "$ROOT" -name "appsettings.json.bak.*" -type f -delete 2>/dev/null || true

echo ""
echo "==> [4/8] Updating .gitignore"
cat > "$ROOT/.gitignore.new" <<'GITIGNORE'
# .NET build output
bin/
obj/
*.user
*.suo
*.userosscache
*.sln.docstates
.vs/

# IDE
.vscode/
.idea/
*.swp
*~

# OS
.DS_Store
Thumbs.db

# Backups & temp
*.bak
*.bak.*
*.backup
*.tmp
*.log

# Local-only setup helpers (not part of the app)
dotnet-install.sh
postgresql.conf
1.txt

# Optional: keep environment-specific settings local only
# appsettings.Development.json
# appsettings.Local.json
GITIGNORE

# Merge with existing .gitignore if present, dedupe
if [ -f "$ROOT/.gitignore" ]; then
  cat "$ROOT/.gitignore" "$ROOT/.gitignore.new" | awk '!seen[$0]++' > "$ROOT/.gitignore.merged"
  mv "$ROOT/.gitignore.merged" "$ROOT/.gitignore"
  rm "$ROOT/.gitignore.new"
else
  mv "$ROOT/.gitignore.new" "$ROOT/.gitignore"
fi
echo "    ✅ .gitignore updated"

echo ""
echo "==> [5/8] Untrack any junk that may already be tracked"
# These commands are safe to run even if the file isn't tracked
git rm -r --cached --ignore-unmatch \
  bin/ obj/ \
  '**/bin/' '**/obj/' \
  .vs/ .vscode/ \
  '*.bak' '**/*.bak' \
  '**/appsettings.json.bak.*' \
  dotnet-install.sh \
  postgresql.conf \
  1.txt \
  >/dev/null 2>&1 || true

echo ""
echo "==> [6/8] Branch setup: $BRANCH"
CUR="$(git rev-parse --abbrev-ref HEAD)"
if [ "$CUR" = "$BRANCH" ]; then
  echo "    Already on $BRANCH"
elif git rev-parse --verify --quiet "$BRANCH" >/dev/null; then
  git checkout "$BRANCH"
  echo "    ✅ Switched to existing branch $BRANCH"
else
  # Branch off whatever the remote default is
  DEFAULT_REMOTE_BRANCH="$(git remote show origin 2>/dev/null | sed -n '/HEAD branch/s/.*: //p' || echo main)"
  DEFAULT_REMOTE_BRANCH="${DEFAULT_REMOTE_BRANCH:-main}"
  echo "    Creating $BRANCH from origin/$DEFAULT_REMOTE_BRANCH"
  git checkout -b "$BRANCH" "origin/$DEFAULT_REMOTE_BRANCH" 2>/dev/null || git checkout -b "$BRANCH"
fi

echo ""
echo "==> [7/8] Stage and commit"
git add -A

# Show what's staged
echo ""
echo "    Files staged:"
git diff --cached --name-status | head -40
TOTAL_STAGED="$(git diff --cached --name-only | wc -l | tr -d ' ')"
echo "    ... ($TOTAL_STAGED files total)"
echo ""

if [ "$TOTAL_STAGED" -eq 0 ]; then
  echo "    ⚠️  Nothing staged. Maybe everything is already committed?"
else
  git commit -m "feat: vendor CRUD, parts inventory, sale invoices, customer reports, notifier

Backend
- Domain: Vendor, Part, SaleInvoice, SaleInvoiceItem
- Repositories + Services for each
- VendorsController     /api/admin/vendors    [Admin]      full CRUD
- PartsController       /api/parts            [Staff,Admin] list + low-stock
                        POST                   [Admin only] create
- SalesController       /api/staff/sales/invoices [Staff,Admin] transactional
- ReportsController     /api/staff/reports/*   [Staff,Admin] regulars,
                        high-spenders, pending-credits
- CustomerInvoicesController /api/customer/invoices [Customer] own history
- NotificationWorker (BackgroundService, runs every 6h)
    - emails admins on parts with stock < threshold
    - emails customers with credit invoices > 30 days unpaid
    - 7-day cooldown per invoice
- EmailService (SMTP, falls back to logging)
- EF migration: AddVendorPartSaleInvoiceFeatures (4 new tables)

Auth
- Identity roles seeded (Admin/Staff/Customer)
- JWT now includes role claims so [Authorize(Roles=...)] works
- AuthService promotes registered users to their selected role
- Default seeded admin: admin@vms.local / Admin@123 (change before prod)

UI (Razor views, matches existing dark-sidebar style)
- New _AdminDashboardLayout
- Admin: /admin/dashboard, /admin/vendors, /admin/parts,
         /admin/low-stock, /admin/reports
- Staff: /staff/new-sale (POS), /staff/invoice/{id} (printable),
         /staff/reports, /staff/low-stock
- Customer: /customer/invoices
- Wired existing 'Inventory' and 'Reports' nav buttons in staff layout
  to real pages; orange 'New Sale' CTA links to /staff/new-sale
- Customer layout: 'My Invoices' nav added"

  echo "    ✅ Committed"
fi

echo ""
echo "==> [8/8] Push branch to origin"
git push -u origin "$BRANCH"

echo ""
echo "==========================================================="
echo "✅ Pushed branch '$BRANCH' to origin."
echo ""
echo "Next steps for the repo owner / for you on GitHub:"
echo "  1. Open the repo in browser; you'll see a yellow banner"
echo "     'Compare & pull request'. Click it."
echo "  2. Set the base branch (usually 'main') and write a PR description."
echo "  3. Submit the PR for review."
echo ""
echo "What collaborators do after merge:"
echo "    git pull"
echo "    dotnet ef database update \\"
echo "      --project Vehicle.Infrastructure \\"
echo "      --startup-project VehicleManagement.API"
echo "  EF will apply the new migration to their local DB."
echo ""
echo "⚠️  Reminder: appsettings.json contains your DB password and JWT key."
echo "   For a school project this is OK; for production, untrack it"
echo "   and ship an appsettings.example.json template instead."
echo "==========================================================="