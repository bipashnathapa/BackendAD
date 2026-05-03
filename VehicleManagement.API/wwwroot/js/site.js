(() => {
  const storageKeys = {
    token: "vm_token",
    email: "vm_email",
  };

  function base64UrlToUtf8(base64Url) {
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/") + "===".slice((base64Url.length + 3) % 4);
    try {
      return decodeURIComponent(
        atob(base64)
          .split("")
          .map((c) => "%" + c.charCodeAt(0).toString(16).padStart(2, "0"))
          .join("")
      );
    } catch {
      return "";
    }
  }

  function parseJwt(token) {
    if (!token || typeof token !== "string") return null;
    const parts = token.split(".");
    if (parts.length < 2) return null;
    const json = base64UrlToUtf8(parts[1]);
    if (!json) return null;
    try {
      return JSON.parse(json);
    } catch {
      return null;
    }
  }

  function getUserRoleFromToken(token) {
    const claims = parseJwt(token);
    if (!claims) return "";

    const candidates = [
      "role",
      "roles",
      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role",
    ];

    for (const key of candidates) {
      const val = claims[key];
      if (!val) continue;
      if (Array.isArray(val)) return String(val[0] || "");
      return String(val);
    }

    return "";
  }

  function getUserDisplayNameFromToken(token) {
    const claims = parseJwt(token);
    if (!claims) return "";

    const candidates = [
      "name",
      "unique_name",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
      "email",
    ];

    for (const key of candidates) {
      const val = claims[key];
      if (!val) continue;
      if (Array.isArray(val)) return String(val[0] || "");
      return String(val);
    }

    return "";
  }

  function getToken() {
    return localStorage.getItem(storageKeys.token) || "";
  }

  function setToken(token, email) {
    localStorage.setItem(storageKeys.token, token);
    if (email) localStorage.setItem(storageKeys.email, email);
  }

  function clearAuth() {
    localStorage.removeItem(storageKeys.token);
    localStorage.removeItem(storageKeys.email);
  }

  function authHeader() {
    const token = getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
  }

  function showToast(kind, message) {
    const toast = document.getElementById("toast");
    if (!toast) return;
    toast.classList.remove("ok", "bad", "show");
    toast.classList.add(kind === "ok" ? "ok" : "bad", "show");
    toast.textContent = message;
  }

  async function apiJson(path, options) {
    const res = await fetch(path, {
      headers: {
        "Content-Type": "application/json",
        ...authHeader(),
        ...(options && options.headers ? options.headers : {}),
      },
      ...options,
    });

    const text = await res.text();
    let data;
    try {
      data = text ? JSON.parse(text) : null;
    } catch {
      data = { raw: text };
    }

    if (!res.ok) {
      const msg =
        (data && data.message) ||
        (Array.isArray(data)
          ? data
              .map((e) => e.description || e.code)
              .filter(Boolean)
              .join(", ")
          : null) ||
        `Request failed (${res.status})`;
      const err = new Error(msg);
      err.status = res.status;
      err.data = data;
      throw err;
    }

    return data;
  }

  function bindTopbar() {
    // Kept for compatibility with older pages; no-op for now.
  }

  function wireNav() {
    const token = getToken();
    const role = getUserRoleFromToken(token).toLowerCase();

    const loginLink = document.getElementById("loginLink");
    const registerLink = document.getElementById("registerLink");
    const dashboardLink = document.getElementById("dashboardLink");

    if (!dashboardLink) return;

    if (!token) {
      dashboardLink.style.display = "none";
      if (loginLink) loginLink.style.display = "";
      if (registerLink) registerLink.style.display = "";
      return;
    }

    const nextUrl = role === "staff" ? "/staff" : "/customer";
    dashboardLink.href = nextUrl;
    dashboardLink.style.display = "";
    if (loginLink) loginLink.style.display = "none";
    if (registerLink) registerLink.style.display = "none";
  }

  window.VM = {
    apiJson,
    bindTopbar,
    wireNav,
    setToken,
    clearAuth,
    showToast,
    storageKeys,
    parseJwt,
    getUserRole: () => getUserRoleFromToken(getToken()),
    getUserDisplayName: () => getUserDisplayNameFromToken(getToken()),
  };

  document.addEventListener("DOMContentLoaded", () => {
    wireNav();
  });
})();
