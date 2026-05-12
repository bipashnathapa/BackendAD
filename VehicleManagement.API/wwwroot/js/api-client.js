(function () {
  function authHeaders() {
    const t = window.VM?.getToken?.() || localStorage.getItem("vm_token") || "";
    return t ? { Authorization: "Bearer " + t } : {};
  }
  async function apiFetch(url, opts = {}) {
    const headers = { "Content-Type": "application/json", ...authHeaders(), ...(opts.headers || {}) };
    const res = await fetch(url, { ...opts, headers });
    if (res.status === 401) { window.VM?.clearAuth?.(); location.href = "/login"; throw new Error("Unauthorized"); }
    if (!res.ok) { const text = await res.text(); throw new Error(text || `HTTP ${res.status}`); }
    if (res.status === 204) return null;
    const ct = res.headers.get("content-type") || "";
    return ct.includes("application/json") ? res.json() : res.text();
  }
  function toast(msg, kind = "success") {
    let el = document.querySelector(".toast");
    if (!el) { el = document.createElement("div"); el.className = "toast"; document.body.appendChild(el); }
    el.className = `toast active ${kind}`; el.textContent = msg;
    setTimeout(() => el.classList.remove("active"), 2800);
  }
  function fmtNPR(n) { if (n == null) return "—"; return "NPR " + Number(n).toLocaleString("en-IN", { minimumFractionDigits: 2, maximumFractionDigits: 2 }); }
  function fmtDate(d) { if (!d) return "—"; return new Date(d).toLocaleDateString("en-GB", { day: "2-digit", month: "short", year: "numeric" }); }
  window.API = { fetch: apiFetch, toast, fmtNPR, fmtDate };
})();
