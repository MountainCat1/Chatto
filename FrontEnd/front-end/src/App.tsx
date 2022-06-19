
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css'

import React from 'react';
import logo from './logo.svg';

import Registration from './components/Registration';
import { BrowserRouter as Router, Route, Link, Routes } from "react-router-dom";
import { Navigate } from 'react-router-dom'
import { Cookies } from 'react-cookie';
import Main from './components/Main';

function App() {

  //const cookies = new Cookies();
  //if(cookies.get('Authorization Token') != null){
  //  return (<><Navigate to="/register"/></>)
  //}

  return (
    <div className="App" >
      <Router>
        <Routes>
          <Route path="/register" element={<Registration />} />
          <Route path="/" element={<Main />} />
        </Routes>
      </Router>

    </div>
  );
}

export default App;
