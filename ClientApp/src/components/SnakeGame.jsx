import { useState, useEffect, useCallback, useRef } from 'react'

const CELL = 20
const COLS = 36
const ROWS = 16
const WIDTH = COLS * CELL
const HEIGHT = ROWS * CELL

const DIR = {
  ArrowUp: [0, -1],
  ArrowDown: [0, 1],
  ArrowLeft: [-1, 0],
  ArrowRight: [1, 0],
}

function randomFood(snake) {
  let pos
  do {
    pos = [Math.floor(Math.random() * COLS), Math.floor(Math.random() * ROWS)]
  } while (snake.some(([x, y]) => x === pos[0] && y === pos[1]))
  return pos
}

function SnakeGame({ onClose }) {
  const canvasRef = useRef(null)
  const dirRef = useRef([1, 0])
  const nextDirRef = useRef([1, 0])
  const [gameOver, setGameOver] = useState(false)
  const [score, setScore] = useState(0)
  const [started, setStarted] = useState(false)
  const snakeRef = useRef([[5, 8], [4, 8], [3, 8]])
  const foodRef = useRef(randomFood(snakeRef.current))
  const gameOverRef = useRef(false)
  const startedRef = useRef(false)

  const draw = useCallback(() => {
    const ctx = canvasRef.current?.getContext('2d')
    if (!ctx) return

    // Background
    ctx.fillStyle = '#0a0a14'
    ctx.fillRect(0, 0, WIDTH, HEIGHT)

    // Subtle grid dots
    ctx.fillStyle = 'rgba(255,255,255,0.03)'
    for (let x = 0; x < COLS; x++) {
      for (let y = 0; y < ROWS; y++) {
        ctx.beginPath()
        ctx.arc(x * CELL + CELL / 2, y * CELL + CELL / 2, 1, 0, Math.PI * 2)
        ctx.fill()
      }
    }

    // Food with glow
    const [fx, fy] = foodRef.current
    ctx.shadowColor = '#f43f5e'
    ctx.shadowBlur = 16
    ctx.fillStyle = '#f43f5e'
    ctx.beginPath()
    ctx.arc(fx * CELL + CELL / 2, fy * CELL + CELL / 2, CELL / 2 - 2, 0, Math.PI * 2)
    ctx.fill()
    ctx.shadowBlur = 0

    // Snake with gradient
    const len = snakeRef.current.length
    snakeRef.current.forEach(([x, y], i) => {
      const t = i / Math.max(len - 1, 1)
      const r = Math.round(167 - t * 60)
      const g = Math.round(139 - t * 80)
      const b = Math.round(250 - t * 40)
      ctx.fillStyle = `rgb(${r},${g},${b})`
      ctx.shadowColor = i === 0 ? '#a78bfa' : 'transparent'
      ctx.shadowBlur = i === 0 ? 16 : 0
      ctx.beginPath()
      ctx.roundRect(x * CELL + 2, y * CELL + 2, CELL - 4, CELL - 4, i === 0 ? 6 : 4)
      ctx.fill()
    })
    ctx.shadowBlur = 0
  }, [])

  const tick = useCallback(() => {
    if (gameOverRef.current || !startedRef.current) return

    dirRef.current = nextDirRef.current
    const [dx, dy] = dirRef.current
    const snake = snakeRef.current
    const [hx, hy] = snake[0]
    const nx = hx + dx
    const ny = hy + dy

    if (nx < 0 || nx >= COLS || ny < 0 || ny >= ROWS || snake.some(([x, y]) => x === nx && y === ny)) {
      gameOverRef.current = true
      setGameOver(true)
      return
    }

    const newSnake = [[nx, ny], ...snake]
    const [fx, fy] = foodRef.current

    if (nx === fx && ny === fy) {
      foodRef.current = randomFood(newSnake)
      setScore(s => s + 10)
    } else {
      newSnake.pop()
    }

    snakeRef.current = newSnake
    draw()
  }, [draw])

  const restart = useCallback(() => {
    snakeRef.current = [[5, 8], [4, 8], [3, 8]]
    dirRef.current = [1, 0]
    nextDirRef.current = [1, 0]
    foodRef.current = randomFood(snakeRef.current)
    gameOverRef.current = false
    startedRef.current = true
    setGameOver(false)
    setScore(0)
    setStarted(true)
    draw()
  }, [draw])

  useEffect(() => { draw() }, [draw])

  useEffect(() => {
    const interval = setInterval(tick, 100)
    return () => clearInterval(interval)
  }, [tick])

  useEffect(() => {
    const handler = (e) => {
      if (DIR[e.key]) {
        e.preventDefault()
        const [dx, dy] = DIR[e.key]
        const [cx, cy] = dirRef.current
        if (dx + cx !== 0 || dy + cy !== 0) {
          nextDirRef.current = [dx, dy]
        }
        if (!startedRef.current) {
          startedRef.current = true
          setStarted(true)
        }
      }
    }
    window.addEventListener('keydown', handler)
    return () => window.removeEventListener('keydown', handler)
  }, [])

  return (
    <div className="card rounded-3xl p-5 mb-6 text-center">
      <div className="flex items-center justify-between mb-3">
        <span className="text-xs font-bold uppercase tracking-widest" style={{ color: 'var(--accent)' }}>
          Snake
        </span>
        <span className="text-sm font-bold tabular-nums">{score}</span>
        <button
          onClick={onClose}
          className="w-7 h-7 rounded-full border-none cursor-pointer flex items-center justify-center text-sm transition-all"
          style={{ background: 'var(--toggle-bg)', color: 'var(--text-dim)' }}
        >
          ✕
        </button>
      </div>
      <canvas
        ref={canvasRef}
        width={WIDTH}
        height={HEIGHT}
        className="rounded-2xl mx-auto block w-full"
        style={{
          height: 'auto',
          aspectRatio: `${WIDTH}/${HEIGHT}`,
          border: '1px solid var(--card-border)',
        }}
      />
      {!started && !gameOver && (
        <p className="text-secondary text-sm mt-4 animate-pulse">
          Нажми стрелку, чтобы начать
        </p>
      )}
      {gameOver && (
        <div className="mt-4">
          <p className="text-sm font-semibold mb-3" style={{ color: '#f43f5e' }}>
            Игра окончена — {score} очков
          </p>
          <button
            onClick={restart}
            className="px-6 py-2.5 rounded-full text-sm font-semibold border-none cursor-pointer transition-all hover:scale-105"
            style={{ background: 'var(--accent)', color: '#fff' }}
          >
            Ещё раз
          </button>
        </div>
      )}

      {/* Mobile controls */}
      <div className="mt-4 flex flex-col items-center gap-1.5 sm:hidden">
        <button
          onClick={() => { nextDirRef.current = [0, -1]; if (!startedRef.current) { startedRef.current = true; setStarted(true) } }}
          className="w-14 h-11 rounded-xl border-none text-lg cursor-pointer transition-colors"
          style={{ background: 'var(--toggle-bg)', color: 'var(--text)' }}
        >↑</button>
        <div className="flex gap-1.5">
          {[['←', [-1, 0]], ['↓', [0, 1]], ['→', [1, 0]]].map(([icon, dir]) => (
            <button
              key={icon}
              onClick={() => { nextDirRef.current = dir; if (!startedRef.current) { startedRef.current = true; setStarted(true) } }}
              className="w-14 h-11 rounded-xl border-none text-lg cursor-pointer transition-colors"
              style={{ background: 'var(--toggle-bg)', color: 'var(--text)' }}
            >{icon}</button>
          ))}
        </div>
      </div>
    </div>
  )
}

export default SnakeGame
