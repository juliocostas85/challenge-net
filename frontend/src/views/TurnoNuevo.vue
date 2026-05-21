<template>
  <div class="card" style="max-width: 560px">
    <h2>Nuevo turno</h2>
    <form @submit.prevent="guardar">
      <div class="form-group">
        <label>Paciente</label>
        <select v-model="form.pacienteId">
          <option value="">Seleccioná un paciente</option>
          <option v-for="p in pacientes" :key="p.id" :value="p.id" :disabled="p.bloqueado">
            {{ p.nombreCompleto }} — DNI {{ p.dni }}{{ p.bloqueado ? ' 🔒 (bloqueado)' : '' }}
          </option>
        </select>
      </div>
      <div class="form-group">
        <label>Médico</label>
        <select v-model="form.medicoId">
          <option value="">Seleccioná un médico</option>
          <option v-for="m in medicos" :key="m.id" :value="m.id">
            {{ m.nombreCompleto }} — {{ m.especialidad }}
          </option>
        </select>
      </div>
      <div class="form-group">
        <label>Fecha y hora</label>
        <input type="datetime-local" v-model="form.fechaHora" />
      </div>
      <div class="form-group">
        <label>Motivo</label>
        <input type="text" v-model="form.motivo" placeholder="Motivo de la consulta" />
      </div>
      <button type="submit">Confirmar turno</button>
    </form>
  </div>
</template>

<script>
import { turnosApi, pacientesApi, medicosApi } from '../services/api'

export default {
  name: 'TurnoNuevo',
  data() {
    return {
      form: {
        pacienteId: '',
        medicoId: '',
        fechaHora: '',
        motivo: ''
      },
      pacientes: [],
      medicos: []
    }
  },
  async mounted() {
    try {
      const [pRes, mRes] = await Promise.all([pacientesApi.getAll(), medicosApi.getAll()])
      this.pacientes = pRes.data
      this.medicos = mRes.data
    } catch {
      alert('Error al procesar la solicitud')
    }
  },
  methods: {
    async guardar() {
      if (!this.form.pacienteId) return alert('Seleccioná un paciente')
      if (!this.form.medicoId)   return alert('Seleccioná un médico')
      if (!this.form.fechaHora)  return alert('Ingresá la fecha y hora del turno')

      try {
        await turnosApi.create({
          pacienteId: Number(this.form.pacienteId),
          medicoId: Number(this.form.medicoId),
          fechaHora: this.form.fechaHora,
          motivo: this.form.motivo
        })
        this.$router.push('/turnos')
      } catch (e) {
        alert(e.response?.data?.mensaje || 'Error al crear el turno')
      }
    }
  }
}
</script>
