import GoogleAuth from "./GoogleAuth";
import 'bootstrap/dist/css/bootstrap.min.css'
import React from "react";
import Form from "./Form";
import '../index.css';

export default function () {
    return (
        <div className="card p-2 m-auto flex-d flex-column shadow-lg" 
        style={
            {
                width: 300,
                height: 550
            }
        }>
            <h2>Chatto Registration</h2>
            <Form/>
            <hr/>
            <GoogleAuth/>
        </div>
    )
}
