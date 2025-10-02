import React from 'react';
import { User } from '../types';

interface HeaderProps {
  user: User | null;
  onLogout: () => void;
  onCreatePost: () => void;
}

const Header: React.FC<HeaderProps> = ({ user, onLogout, onCreatePost }) => {
  return (
    <header className="header">
      <div className="header-content">
        <h1>Мой Блог</h1>
        <nav className="header-nav">
          {user ? (
            <>
              <button onClick={onCreatePost} className="create-post-button">
                Создать пост
              </button>
              <span className="user-info">
                Привет, {user.username}!
              </span>
              <button onClick={onLogout} className="logout-button">
                Выйти
              </button>
            </>
          ) : (
            <span>Добро пожаловать в блог!</span>
          )}
        </nav>
      </div>
    </header>
  );
};

export default Header;
