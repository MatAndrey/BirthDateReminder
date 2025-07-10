import { useParams, useOutletContext } from "react-router";
import Popup from "./Popup";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import AuthService from "./AuthService";

function BirthdayEdit() {
    const { id } = useParams<{ id: string | undefined }>();
    const navigate = useNavigate();
    const [date, setDate] = useState(new Date().toJSON().slice(0, 10));
    const [name, setName] = useState("");
    const [imageFile, setImageFile] = useState<File | null>(null);
    const [imagePath, setImagePath] = useState("")

    const loadBirthdays: Function = useOutletContext();

    useEffect(() => {
        const fetchBirthday = async () => {
            try {
                const response = await AuthService.fetchWithAuth(`/api/birthday/${id}`)
                const data = await response.json();
                setDate(data.birthDate)
                setName(data.name)
                setImagePath(data.imagePath)
            } catch (err) {
                console.error(err);
            }
        };

        if (id) {
            fetchBirthday();
        }            
    }, [id]);

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
