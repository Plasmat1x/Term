import { useState } from "react";
import { useNavigate } from "react-router";
import axios from "axios";
import { Button, Form, Input } from "antd";
import '@ant-design/v5-patch-for-react-19';

const CreatePost = () => {
  const [formData, setFormData] = useState({ title: "", content: "" });
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("token");
      await axios.post("https://localhost:7003/api/posts/create", formData, {
        headers: { Authorization: `Bearer ${token}` },
      });
      navigate("/");
    } catch (err) {
      alert("Ошибка при создании поста");
    }
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100">
      <form className="bg-white p-6 rounded-lg shadow-lg w-96" onSubmit={handleSubmit}>
        <h2 className="text-2xl font-bold mb-4 text-center">Create post</h2>
        <Input
          type="text"
          name="title"
          placeholder="Title"
          className="w-full p-2 mb-2 border rounded"
          onChange={handleChange}
        />
        <textarea
          name="content"
          placeholder="Content"
          className="w-full p-2 mb-2 border rounded"
          onChange={handleChange}
        />
        <Button htmlType="submit" className="w-full bg-blue-500 text-white p-2 rounded">
          Post
        </Button>
      </form>
    </div>
  );
};

export default CreatePost;