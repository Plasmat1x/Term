import { useContext } from "react";
import { useNavigate } from "react-router";
import { AuthContext } from "../AuthContext";
import PostList from "../components/PostList";
import { Button } from "antd";
import '@ant-design/v5-patch-for-react-19';

const Home = () => {
  const { user } = useContext(AuthContext);
  const navigate = useNavigate();

  return (
    <div className="flex flex-col items-center">
      <h1 className="text-3xl font-bold">Main page</h1>
      {user && (
        <Button
          className="mt-4 px-4 py-2 bg-blue-500 text-white rounded"
          onClick={() => navigate("/create-post")}
        >
          Create post
        </Button>
      )}
      <PostList />
    </div>
  );
};

export default Home
