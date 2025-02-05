import { useContext } from "react";
import { AuthContext } from "../AuthContext.jsx";
import { Link } from "react-router";
import { Button } from "antd";
import '@ant-design/v5-patch-for-react-19';

const Footer = () => {
  const { user, logout } = useContext(AuthContext);

  return (
    <footer className="bg-black-500">
      <p>footer</p>
    </footer>
  );
};

export default Footer;