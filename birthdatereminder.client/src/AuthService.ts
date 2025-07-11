class AuthService {
    async login(email: string, password: string) {
        const res = await fetch("/api/auth/login", {
            method: "POST",
            body: JSON.stringify({ email, password }),
            headers: {
                "Content-Type": "application/json"
            }
        })
        const data = await res.json();
        localStorage.setItem("userToken", data.token)
        return res;
    }

    async register(email: string, password: string, confirmPassword: string) {
        const res = await fetch("/api/auth/register", {
            method: "POST",
            body: JSON.stringify({ email, password, confirmPassword }),
            headers: {
                "Content-Type": "application/json"
            }
        })
        const data = await res.json();
        localStorage.setItem("userToken", data.token)
        return [res, data];
    }

    logout() {
        localStorage.removeItem('userToken');
    }

    getCurrentUserToken() {
        return localStorage.getItem('userToken');
    }

    isAuthenticated() {
        const token = this.getCurrentUserToken();
        return !!token;
    }

    async fetchWithAuth(url: string, opts: RequestInit = {}): Promise<Response> {
        const headers = {
            Authorization: `Bearer ${this.getCurrentUserToken()}`,
            ...opts.headers
        };

        let response = await fetch(url, { ...opts, headers });

        if (response.status === 401) {
            this.logout();
            window.location.reload();
        }

        return response;
    }
}

export default new AuthService()