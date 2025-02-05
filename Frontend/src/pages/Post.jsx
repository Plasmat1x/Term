import { useEffect, useState, useContext } from "react";
import { useParams, useNavigate } from "react-router";
import { Spin, Button } from "antd";
import { AuthContext } from "../AuthContext.jsx";
import axios from "axios";
import Typography from "antd/es/typography/Typography";
import CommentList from "../components/CommentList";

const Post = () => {
  const { id } = useParams();
  const { user } = useContext(AuthContext);
  const navigate = useNavigate();
  const [post, setPost] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPost = async () => {
      try {
        const response = await axios.get(`https://localhost:7003/api/posts/${id}`);
        setPost(response.data);
      } catch (err) {
        console.error("Post loading error", err);
        navigate("/");
      }
    };
    fetchPost();
  }, [id, navigate]);

  const handleDelete = async (id) => {
    try {
      await axios.delete(`https://localhost:7003/api/posts/${id}`, {
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      });
      setPost(null);
      navigate("/");
    } catch (err) {
      console.error("Error deleting post", err);
    } finally {
      setLoading(false);
    }
  };

  if (!post) return <Spin />;

  return (
    <div className="container mx-auto p-4">
      <Button onClick={() => navigate(-1)} className="mb-4 text-blue-600">⬅️ Back</Button>
      {user && (user.role === "Admin" || user.role === "Moderator") && (
        <Button onClick={() => handleDelete(post.postId)} className="mb-4 text-blue-600">🗑️ Delete</Button>
      )}
      <h2 className="text-3xl font-bold">{post.title}</h2>
      <Typography className="text-gray-500">Author: <strong className="text-xl">{post.author}</strong> | {new Date(post.createdAt).toLocaleDateString()} {new Date(post.createdAt).toLocaleTimeString()}</Typography>
      <div className="mt-4 p-4 bg-white shadow-md rounded-lg">
        <Typography>{post.content}</Typography>
      </div>
      <CommentList postId={post.postId} />
    </div>
  );
};

export default Post