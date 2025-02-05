import { useState } from "react";
import { useNavigate } from "react-router";
import { Button, Input } from "antd";
import axios from "axios";
import '@ant-design/v5-patch-for-react-19';

const Register = () => {
  const [formData, setFormData] = useState({
    username: "",
    email: "",
    password: "",
  });

  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    try {
      await axios.post("https://localhost:7003/api/account/register", formData);
      navigate("/login");
    } catch (err) {
      console.error("Registration error:", err.response?.data);

      const errorMessage = err.response?.data?.errors
        ? Object.values(err.response.data.errors).flat().join(", ") // Объединяем все ошибки в строку
        : err.response?.data?.message || "Registration error";

      setError(errorMessage);
    }
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100">
      <form className="bg-white p-6 rouded-lg shadow-lg w-96" onSubmit={handleSubmit}>
        <h2 className="text-2xl font-bold mb-4 text-center">Registration</h2>
        {error && <p className="text-red-500">{error}</p>}

        <Input
          type="text"
          name="username"
          placeholder="User name"
          className="w-full p-2 mb-2 border rounded"
          onChange={handleChange}
        />
        <Input
          type="text"
          name="email"
          placeholder="Email"
          className="w-full p-2 mb-2 border rounded"
          onChange={handleChange}
        />
        <Input
          type="password"
          name="password"
          placeholder="Password"
          className="w-full p-2 mb-2 border rounded"
          onChange={handleChange}
        />
        <Button htmlType="submit" className="w-full bg-blue-500 text-white p2 rounded">
          Register
        </Button>
        <Button onClick={() => navigate(-1)} className="mb-4 text-blue-600">← Back</Button>
      </form>
    </div>
  );
};

export default Register;