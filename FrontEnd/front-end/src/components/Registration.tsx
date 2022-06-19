import GoogleAuth from "./GoogleAuth";

import React from "react";
import Form from "./Form";
import '../index.css';
import { Navigate, Router } from "react-router-dom";
import { Cookies } from "react-cookie";
import { useNavigate } from 'react-router-dom';

export default function () {

    const navigate = useNavigate();

    const cookies = new Cookies();
    const authCookie = cookies.get("Authorization")

    console.log("Authorization cookie: " + authCookie);
    
    const goBackToMainPage = authCookie != null && authCookie != undefined;

    return (
        <>
            {
                goBackToMainPage ?
                    <>
                        <Navigate to="/"/>
                    </>
                    : 
                    <div className="card p-2 m-auto flex-d flex-column shadow-lg"
                        style={
                            {
                                width: 300,
                                height: 550
                            }
                        }>
                        <h2>Chatto Registration</h2>
                        <Form />
                        <hr />
                        <GoogleAuth />
                    </div>
            }
        </>

    )
}
