import { useContext } from "react";
import { AuthContext } from "../AuthContext.jsx";
import { Link } from "react-router";
import { Button } from "antd";
import '@ant-design/v5-patch-for-react-19';

const Header = () => {
  const { user, logout } = useContext(AuthContext);

  return (
    <header className="p-4 bg-blue-500 text-white flex justify-between">
      <Link to="/">
        <h1 className="text-xl text-white hover:text-blue-200">
          Forum
        </h1>
      </Link>
      <div>
        {user ? (
          <>
            <span className="mr-4">Hello, {user.name}!</span>
            <Button onClick={logout} className="bg-blue-500 hover:bg-red-500 px-4 py-2 rounded">
              Logout
            </Button>
          </>
        ) : (
          <>
            <Link to="/login" className="mr-4"><Button>Login</Button></Link>
            <Link to="/register"><Button>Register</Button></Link>
          </>
        )}
      </div>
    </header>
  );
};

export default Header;