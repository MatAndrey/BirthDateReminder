import Popup from "./Popup";
import { useEffect, useState } from "react";
import AuthService from "./AuthService";

function Settings() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");

    useEffect(() => {
        const fetchSettings = async () => {
            try {
                const response = await AuthService.fetchWithAuth(`/api/settings`)
                const data = await response.json();
                if (response.ok) {
                    setEmail(data.email)
                }
            } catch (err) {
                console.error(err);
            }
        };

        fetchSettings();
    }, [])

    const changePassword = async (event: React.FormEvent) => {
        event.preventDefault();
        const resp = await AuthService.fetchWithAuth(`/api/settings/change-password`,
            {
                method: "PUT",
                body: JSON.stringify({
                    oldPassword: password,
                    newPassword
                }),
                headers: {
                    "Content-Type": "application/json"
                }
            })
        if (resp.ok) {
            setPassword("");
            setNewPassword("");
            alert("Пароль успешно изменён")
        } else {
            alert("Ошибка при изменении пароля")
            console.log(await resp.json())
        }
    }

    const changeEmail = async (event: React.FormEvent) => {
        event.preventDefault();
        const resp = await AuthService.fetchWithAuth(`/api/settings/change-email`,
            {
                method: "PUT",
                body: JSON.stringify({
                    newEmail: email,
                    token: AuthService.getCurrentUserToken()
                }),
                headers: {
                    "Content-Type": "application/json"
                }
            })
        if (resp.ok) {
            alert("Почта успешно изменена")
        } else {
            alert("Ошибка при изменении почты")
            console.log(await resp.json())
        }
    }

    const logout = (event: React.FormEvent) => {
        event.preventDefault()
        if (!confirm("Выйти из аккаунта?")) return;
        AuthService.logout();
        window.location.reload();
    }

    const deleteAccount = async (event: React.FormEvent) => {
        event.preventDefault();
        const answer = prompt("Вы действительно хотите безвозвратно удалить свой аккаунт? Введите \"Удалить\" для подтверждения");
        if (answer == "Удалить") {
            const resp = await AuthService.fetchWithAuth(`/api/settings`,
                {
                    method: "DELETE"
                })
            if (resp.ok) {
                alert("Аккаунт успешно удалён")
                AuthService.logout();
                window.location.reload();
            } else {
                alert("Ошибка при удалении аккаунта")
                console.log(await resp.json())
            }
        }
    }

    const testEmail = async (event: React.FormEvent) => {
        event.preventDefault();
        const resp = await AuthService.fetchWithAuth(`/api/settings/test-email`,
            {
                method: "POST"
            })
        if (resp.ok) {
            alert("Тестовое письмо отправлено")
        } else {
            alert("Ошибка при отправке письма")
            console.log(await resp.json())
        }
    }

    return <Popup>
        <h2>Настройки</h2>
        <form>
            <label htmlFor="email">Электронная почта</label>
            <input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="user@example.com"
                required
            />
            <button className="primary button" onClick={changeEmail}>Изменить почту</button>
            <button className="button" onClick={testEmail}>Отправить тестовое письмо</button>
        </form>
        <form>
            <label htmlFor="password">Смена пароля</label>
            <input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="текущий пароль"
                required
            />
            <input
                id="newPassword"
                type="password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                placeholder="новый пароль"
                required
            />
            <button className="primary button" onClick={ changePassword }>Изменить пароль</button>
        </form>
        <form>
            <button className="button" onClick={logout}>Выйти из аккаунта</button>
        </form>
        <form>
            <button className="button" onClick={deleteAccount}>Удалить аккаунт</button>
        </form> 
    </Popup>
}

export default Settings;
