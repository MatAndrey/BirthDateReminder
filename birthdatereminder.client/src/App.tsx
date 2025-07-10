import { Routes, Route } from "react-router";
import BirthdaysPage from "./BirthdaysPage";
import Register from "./Register";
import Login from "./Login";
import BirthdayEdit from "./BirthdayEdit";
import { Navigate } from "react-router";
import AuthService from "./AuthService";
import "./App.css"
import { Link } from "react-router";
import Settings from "./Settings";

function App() {
    return <>
        <header>
            <nav>
                <Link to="/">
                    <h1>ПОЗДРАВЛЯТОР</h1>
                </Link>
                <Link to="/birthdays/settings">
                    Настройки
                </Link>
            </nav>
        </header>
        <main>
            <Routes>
                {AuthService.isAuthenticated() ?
                    <>
                        <Route path="/birthdays" element={<BirthdaysPage />}>
                            <Route path=":id" element={<BirthdayEdit />} />
                            <Route path="create" element={<BirthdayEdit />} />
                            <Route path="settings" element={<Settings />} />
                        </Route>
                        <Route path="*" element={<Navigate to="/birthdays" />} />
                    </>
                    :
                    <>
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route path="*" element={<Navigate to="/login" />} />
                    </>
                }
            </Routes>
        </main>
    </>
}

export default App;