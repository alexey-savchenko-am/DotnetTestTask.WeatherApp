function Loader() {
  return (
    <div className="flex flex-col items-center justify-center min-h-[50vh] gap-4">
      <div className="w-10 h-10 border-3 rounded-full animate-spin-slow" style={{ borderColor: 'var(--card-border)', borderTopColor: 'var(--text-primary)' }} />
      <p className="text-secondary">Загрузка...</p>
    </div>
  )
}

export default Loader
