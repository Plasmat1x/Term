import { createContext, useState, useEffect } from "react";

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [role, setRole] = useState(null);
  const [token, setToken] = useState(null);

  useEffect(() => {
    const storedUser = localStorage.getItem("user", user);
    const storedRole = localStorage.getItem("role", role);
    const storedToken = localStorage.getItem("token", token);

    if (storedUser && storedToken && storedRole) {
      setUser(JSON.parse(storedUser));
      setRole(JSON.parse(storedRole));
      setToken((storedToken));
    }
  }, []);

  const login = (userData, userRole, authToken) => {

    console.log(userData);
    console.log(userRole);
    console.log(authToken);

    setUser(userData);
    setRole(userRole);
    setToken(authToken);
    localStorage.setItem("user", JSON.stringify(userData));
    localStorage.setItem("role", JSON.stringify(userRole));
    localStorage.setItem("token", authToken);
  };

  const logout = () => {
    setUser(null);
    setRole(null);
    setToken(null);
    localStorage.removeItem("user");
    localStorage.removeItem("role");
    localStorage.removeItem("token");
  };

  return (
    <AuthContext.Provider value={{ user, role, token, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};