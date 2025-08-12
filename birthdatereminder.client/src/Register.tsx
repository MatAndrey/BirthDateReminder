import { useState, useTransition } from "react";
import "./Auth.css"
import { Link, useNavigate } from "react-router";
import AuthService from "./AuthService";
function Register() {
    const [pending, startTransition] = useTransition();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("")
    const [errors, setErrors] = useState<string[]>([]);

    const navigate = useNavigate();

    function sendForm(event: React.FormEvent) {
        event.preventDefault();
        startTransition(async () => {
            setErrors([]);
            const [resp, data] = await AuthService.register(email, password, confirmPassword);
            if (resp.ok) {
                navigate("/birthdays")
                navigate(0)
                return
            }
            if (data.errors) {
                if (Array.isArray(data.errors))
                    setErrors(data.errors)
                else {
                    for (let key in data.errors) {
                        setErrors(prev => [...prev, data.errors[key]])
                    }
                }
            }
        });
    }
    return (
        <form className="login" onSubmit={sendForm}>
            <h2>Регистрация</h2>
            <input
                className="field"
                type='email'
                placeholder='email'
                name='email'
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)} />
            <input
                className="field"
                type='password'
                placeholder='пароль'
                name='password'
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)} />
            <input
                className="field"
                type='password'
                placeholder='повторите пароль'
                name='password'
                required
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)} />
            {errors && errors.map((el, i) => <p className="error-message" key={i}>{el}</p>)}
            <button className="button primary" disabled={pending}>
                Зарегистрироваться
            </button>
            <span>
                Уже есть аккаунт? <Link to='/login'>Войти</Link>
            </span>
        </form>
    );
}

export default Register;