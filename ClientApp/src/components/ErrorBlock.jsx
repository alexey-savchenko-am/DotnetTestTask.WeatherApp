function ErrorBlock({ message, onRetry }) {
  return (
    <div className="text-center py-12 px-4">
      <p className="text-lg mb-4" style={{ opacity: 0.9 }}>{message || 'Что-то пошло не так'}</p>
      <button
        onClick={onRetry}
        className="card px-7 py-2.5 rounded-xl text-base cursor-pointer transition-all hover:scale-105"
        style={{ color: 'var(--text-primary)' }}
      >
        Повторить
      </button>
    </div>
  )
}

export default ErrorBlock
