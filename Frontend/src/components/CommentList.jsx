import { useEffect, useState, useContext } from "react";
import axios from "axios";
import { AuthContext } from "../AuthContext.jsx";
import { Divider, Button, Spin, Typography } from "antd";

const CommentList = ({ postId }) => {
  const { user } = useContext(AuthContext);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchComments = async () => {
      try {
        const response = await axios.get(`https://localhost:7003/api/comments/${postId}`);
        console.log(response.data);
        setComments(response.data);
      } catch (err) {
        console.error("Error loading comments", err);
      }
      finally {
        setLoading(false);
      }
    };
    fetchComments();
  }, [postId]);

  const handleAddComment = async () => {
    if (!newComment.trim()) return;
    try {
      const response = await axios.post(`https://localhost:7003/api/comments/${postId}`, {
        content: newComment,
      }, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      });
      setComments([response.data, ...comments]);
      setNewComment("");
    } catch (err) {
      console.error("Error adding comment", err);
    }
    finally {
      setLoading(false);
    }
  };

  const handleDelete = async (commentId) => {
    try {
      await axios.delete(`https://localhost:7003/api/comments/${commentId}`, {
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      });
      setComments(comments.filter(c => c.postId !== postId));
    } catch (err) {
      console.error("Error deleting comment", err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <Spin className="object-center"></Spin>
  }

  return (
    <div className="mt-4">
      <h3 className="text-lg font-bold">Comments</h3>
      <Divider />
      {user && (
        <div className="mt-2">
          <textarea
            type="text"
            value={newComment}
            onChange={(e) => setNewComment(e.target.value)}
            placeholder="Write a comment..."
            className="border p-2 w-full rounded"
          />
          <Button onClick={handleAddComment} className="bg-blue-500 text-white p-2 mt-2">Add Comment</Button>
        </div>
      )}
      <ul>
        {comments.map(comment => (
          <li key={comment.id} className="border-b p-2">
            <Typography><strong>{comment.author}</strong>: {comment.content}</Typography>
            {user && (user?.name === comment.author || user.role === "Admin" || user.role === "Moderator") && (
              <Button className="text-red-500" onClick={() => handleDelete(comment.commentId)}>Delete</Button>
            )}
            <Typography>{new Date(comment.createdAt).toLocaleDateString()} {new Date(comment.createdAt).toLocaleTimeString()}</Typography>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default CommentList;

