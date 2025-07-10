import type { JSX } from "react";
import "./Popup.css"
import { useNavigate } from "react-router";

function Popup(
    { children }: 
    { children: string | JSX.Element | JSX.Element[] }) {
    const navigate = useNavigate();
    
    function onClick() {
        navigate(-1);
    }

    if (!children) return null;

    return (
        <div className="popup-background" onMouseDown={ onClick } >
            <div className="popup" onMouseDown={ e => e.stopPropagation() }>
                { children }
            </div>
        </div>
    );
}

export default Popup;