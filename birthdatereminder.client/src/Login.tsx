import { useState, useTransition } from "react";
import "./Auth.css"
import { Link } from "react-router";
import AuthService from "./AuthService";
import { useNavigate } from "react-router";

function Login() {
    const [pending, startTransition] = useTransition();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [errors, setErrors] = useState<string[]>([]);

    const navigate = useNavigate();

    function sendForm(event: React.FormEvent) {
        event.preventDefault();
        startTransition(async () => {
            setErrors([]);
            const resp = await AuthService.login(email, password);
            if (resp.errors) {
                if (Array.isArray(resp.errors))
                    setErrors(resp.errors)
                else {
                    for (let key in resp.errors) {
                        setErrors(prev => [...prev, resp.errors[key]])
                    }
                }
            } else {
                navigate("/birthdays")
                navigate(0)
            }
        });
    }

    return (
        <form className="login" onSubmit={sendForm}>
            <h2>Вход</h2>
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
            {errors && errors.map((el, i) => <p className="error-message" key={ i }>{el}</p>)}
            <button className="primary button" disabled={pending}>
                Войти
            </button>
            <span>
                Ещё нет аккаунта? <Link to='/register'>Зарегистрироваться</Link>
            </span>
        </form>
    );
}

export default Login;