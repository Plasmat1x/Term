import { useContext, useState } from "react";
import { useNavigate } from "react-router";
import { AuthContext } from "../AuthContext.jsx";
import { Button, Input } from "antd";
import '@ant-design/v5-patch-for-react-19';

const Login = () => {
  const navigate = useNavigate();
  const { login } = useContext(AuthContext);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const handleLogin = async () => {
    setLoading(true);
    try {
      const response = await fetch("https://localhost:7003/api/account/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      if (!response.ok) {
        throw new Error("Login error");
      }

      const data = await response.json();
      const userData = { name: data.name };
      const userRole = { role: data.role };

      localStorage.setItem("token", data.token);
      localStorage.setItem("user", JSON.stringify(userData));
      localStorage.setItem("role", JSON.stringify(userRole));

      login(userData, userRole, data.token);
      navigate("/");
    } catch (error) {
      alert(error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100 ">
      <div className="bg-white p-6 rounded-lg shadow-lg w-96">
        <h2 className="text-2xl font-bold mb-4 text-center">Login</h2>
        <Input
          type="text"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          placeholder="Имя пользователя"
          className="w-full p-2 mb-2 border rounded"
        />
        <Input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="Пароль"
          className="w-full p-2 mb-4 border rounded"
        />
        <Button
          onClick={handleLogin}
          disabled={loading}
          className="w-full p-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition disabled:bg-gray-400"
        >
          {loading ? "Loggining..." : "Login"}
        </Button>
        <Button onClick={() => navigate(-1)} className="mb-4 text-blue-600">← Back</Button>
      </div>
    </div>
  );
};

export default Login;