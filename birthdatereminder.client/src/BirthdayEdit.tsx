import { useParams, useOutletContext } from "react-router";
import Popup from "./Popup";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import AuthService from "./AuthService";
import { reminderToString } from "./utils";

interface IReminder {
    id: number
    unitsCount: number
    unitsType: number
}

function BirthdayEdit() {
    const { id } = useParams<{ id: string | undefined }>();
    const navigate = useNavigate();
    const [date, setDate] = useState(new Date().toJSON().slice(0, 10));
    const [name, setName] = useState("");
    const [imageFile, setImageFile] = useState<File | null>(null);
    const [imagePath, setImagePath] = useState("");
    const [newReminderNum, setNewReminderNum] = useState(1);
    const [newReminderUnit, setNewReminderUnit] = useState("День");
    const [reminders, setReminders] = useState<IReminder[]>([]);

    const loadBirthdays: Function = useOutletContext();

    useEffect(() => {
        const fetchBirthday = async () => {
            try {
                const response = await AuthService.fetchWithAuth(`/api/birthday/${id}`)
                const data = await response.json();
                setDate(data.birthDate)
                setName(data.name)
                setImagePath(data.imagePath)
                setReminders(data.reminders)
            } catch (err) {
                console.error(err);
            }
        };

        if (id) {
            fetchBirthday();
        }            
    }, [id]);

    const handleCreateReminder = async (e: React.FormEvent) => {
        e.preventDefault();

        const resp = await AuthService.fetchWithAuth(`/api/birthday/reminder`,
            {
                method: "POST",
                body: JSON.stringify({
                    birthdayId: id,
                    reminderUnit: newReminderUnit,
                    reminderNum: newReminderNum
                }),
                headers: {
                    "Content-Type": "application/json"
                }
            })
        if (resp.ok) {
            const data = await resp.json();
            setReminders(data)
        } else {
            alert("Ошибка при добвлении напоминания")
            console.log(await resp.json())
        }
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const formData = new FormData();
            formData.append('Name', name);
            formData.append('BirthDate', date);

            if (imageFile) {
                formData.append('Image', imageFile);
            }

            if (id === undefined) {
                await AuthService.fetchWithAuth(`/api/birthday`, {
                    method: "POST",
                    body: formData
                })
            } else {
                await AuthService.fetchWithAuth(`/api/birthday/${id}`, {
                    method: "PUT",
                    body: formData
                })
            }            

            navigate('/birthdays', { replace: true });
            loadBirthdays()
        } catch (err) {
            console.error(err);
        }
    };

    const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files[0]) {
            const file = e.target.files[0];
            setImageFile(file)

            const reader = new FileReader();
            reader.onload = (event) => {
                if (event.target?.result) {
                    setImagePath(event.target.result as string);
                }
            };
            reader.readAsDataURL(file);
        }
    };

    const handleDelete = async () => {
        if (id) {
            if (!confirm("Вы действительно хотите удалить запись?")) return;
            AuthService.fetchWithAuth(`/api/birthday/${id}`, {
                method: "DELETE"
            })
            navigate('/birthdays');
            loadBirthdays()
        }
    }

    const handleClose = () => {
        navigate('/birthdays');
    };

    const deleteReminder = async (id: number) => {
        if (id) {
            const resp = await AuthService.fetchWithAuth(`/api/birthday/reminder/${id}`, {
                method: "DELETE"
            })
            if (resp.ok) {
                const data = await resp.json();
                setReminders(data)
            } else {
                alert("Ошибка при удалении напоминания")
                console.log(await resp.json())
            }
        }
    }

    return <Popup>
        <h2>{ id === undefined ? "Создание" : "Редактирование" }</h2>
        <form onSubmit={handleSubmit} className="birthday-edit-form">
            <label htmlFor="name">Имя</label>
            <input
                id="name"
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
            />
            <label htmlFor="birthDate">Дата рождения</label>
            <input
                id="birthDate"
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
                required
            />

            <label htmlFor="image">Фото</label>
            <input
                id="image"
                type="file"
                accept="image/*"
                onChange={handleImageChange}
            />
            {imagePath && (
                <div className="image-preview">
                    <img src={imagePath} alt="Preview" />
                </div>
            )}

            {id !== undefined &&
                <>
                <h3>Напоминания</h3>
                <ul className="reminders">
                    {reminders.map(r =>
                        <li>
                            За {r.unitsCount} {reminderToString(r.unitsCount, r.unitsType)} <button type="button" className="button" onClick={() => deleteReminder(r.id)}>Удалить</button>
                        </li>)}
                </ul>
                <div className="reminders-settings">
                    За
                    <input type="number" min="1" max="31" value={newReminderNum} onChange={e => setNewReminderNum(+e.target.value)} />
                    <select value={newReminderUnit} onChange={e => setNewReminderUnit(e.target.value)}>
                        <option>День</option>
                        <option>Неделя</option>
                        <option>Месяц</option>
                    </select>
                    <button type="button" className="button" onClick={handleCreateReminder}>
                        Добавить
                    </button>
                </div>
                </>}

            <div className="form-actions">
                <button type="button" className="button" onClick={handleClose}>
                    Отмена
                </button>
                {id !== undefined && <button type="button" className="button" onClick={handleDelete}>
                    Удалить запись
                </button>}
                
                <button type="submit" className="primary button">
                    Сохранить
                </button>
            </div>
        </form>
    </Popup>
}

export default BirthdayEdit;
