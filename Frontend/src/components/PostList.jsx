import { useEffect, useState } from "react";
import { Link } from "react-router";
import axios from "axios";
import { Spin, Button, Typography, Divider } from "antd";

const PostList = () => {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    const fetchPosts = async () => {
      try {
        const response = await axios.get("https://localhost:7003/api/posts/all");
        setPosts(response.data);
      } catch (err) {
        console.error("Error loading posts", err);
      } finally {
        setLoading(false);
      }
    };
    fetchPosts();
  }, []);

  if (loading) {
    return <Spin className="object-center"></Spin>
  }

  return (
    <div className="container mx-auto p-4">
      {posts.length === 0 ? (
        <Typography>No posts</Typography>
      ) : (
        <div className="space-y-4">
          {posts.map((post) => (
            <div key={post.id} className="p-4 bg-white shadow-md rounded-lg">
              <h3 className="text-xl">{post.title}</h3>
              <Typography>{new Date(post.createdAt).toLocaleDateString()}</Typography>
              <Divider />
              <Typography className="text-gray-600">{post.content.slice(0, 50)}...</Typography>
              <Link to={`/post/${post.postId}`} className="text-xl font-semibold text-blue-600">
                <Button>
                  Read more
                </Button>
              </Link>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default PostList;