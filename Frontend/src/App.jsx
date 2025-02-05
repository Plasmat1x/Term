import { BrowserRouter, Route, Routes } from "react-router";
import './index.css';
import Home from './pages/Home.jsx';
import Post from "./pages/Post.jsx";
import Admin from "./pages/Admin.jsx";
import Register from "./pages/Register.jsx";
import Login from "./pages/Login.jsx";
import Header from "./components/Header.jsx";
import CreatePost from "./pages/CreatePost.jsx";
import Footer from "./components/Footer.jsx";
import { Layout } from "antd";
import '@ant-design/v5-patch-for-react-19';

export default function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Header />
        <main className="p-4">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/post/:id" element={<Post />} />
            <Route path="/admin" element={<Admin />} />
            <Route path="/register" element={<Register />} />
            <Route path="/login" element={<Login />} />
            <Route path="/create-post" element={<CreatePost />} />
          </Routes>
        </main>
        <Footer />
      </Layout>
    </BrowserRouter >
  );
};