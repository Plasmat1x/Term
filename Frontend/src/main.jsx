import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App.jsx";
import { AuthProvider } from "./AuthContext.jsx";

const root = document.getElementById("root");
ReactDOM.createRoot(root).render(
  <AuthProvider>
    <App />
  </AuthProvider>
);