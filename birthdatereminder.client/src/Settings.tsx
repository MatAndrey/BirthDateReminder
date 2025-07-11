import Popup from "./Popup";
import { useEffect, useState } from "react";
import AuthService from "./AuthService";

function Settings() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [notifyInBD, setNotifyInBD] = useState(false)
    const [notifyDayBefore, setNotifyDayBefore] = useState(false)

    useEffect(() => {
        const fetchSettings = async () => {
            try {
                const response = await AuthService.fetchWithAuth(`/api/settings`)
                const data = await response.json();
                if (response.ok) {
                    setEmail(data.email)
                    setNotifyDayBefore(data.notifyDayBefore)
                    setNotifyInBD(data.notifyInBD)
                }
            } catch (err) {
                console.error(err);
            }
        };

        fetchSettings();
    }, [])

    const saveNotifications = async (event: React.FormEvent) => {
        event.preventDefault();
        const resp  = await AuthService.fetchWithAuth(`/api/settings/notifications`,
            {
                method: "PUT",
                body: JSON.stringify({ notifyDayBefore, notifyInBD }),
                headers: {
                    "Content-Type": "application/json"
                }
            })
        if (resp.ok) {
            alert("Настройки успешно сохранены")
        } else {
            alert("Ошибка при сохранении настроек")
            console.log(await resp.json())
        }
    }

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
            <div>
                <input
                    id="notify"
                    type="checkbox"
                    checked={notifyInBD}
                    onChange={ () => setNotifyInBD(!notifyInBD) }
                />
                <label htmlFor="notify">Оповещать в день ДР</label>
            </div>
            <div>
                <input
                    id="notifyDayBefore"
                    type="checkbox"
                    checked={notifyDayBefore}
                    onChange={() => setNotifyDayBefore(!notifyDayBefore)}
                />
                <label htmlFor="notifyDayBefore">Оповещать за день до ДР</label>
            </div>
            <button className="primary button" onClick={saveNotifications}>Сохранить настройки</button>
            <button className=" button" onClick={logout}>Выйти из аккаунта</button>
        </form>
    </Popup>
}

export default Settings;
