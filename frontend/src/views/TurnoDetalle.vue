<template>
  <div>
    <router-link to="/turnos" style="font-size:14px; color:#1a73e8">← Volver a turnos</router-link>
    <div v-if="turno" class="card" style="margin-top: 20px; max-width: 560px">
      <h2>Turno #{{ turno.id }}</h2>
      <div class="detail-row"><span class="label">Paciente</span><span>{{ turno.paciente?.nombreCompleto }}</span></div>
      <div class="detail-row"><span class="label">DNI</span><span>{{ turno.paciente?.dni }}</span></div>
      <div class="detail-row"><span class="label">Médico</span><span>{{ turno.medico?.nombreCompleto }}</span></div>
      <div class="detail-row"><span class="label">Especialidad</span><span>{{ turno.medico?.especialidad }}</span></div>
      <div class="detail-row"><span class="label">Fecha y hora</span><span>{{ formatFecha(turno.fechaHora) }}</span></div>
      <div class="detail-row"><span class="label">Estado</span><span>{{ turno.estado }}</span></div>
      <div class="detail-row"><span class="label">Motivo</span><span>{{ turno.motivo }}</span></div>

      <div style="margin-top: 24px">
        <div class="form-group">
          <label>Cambiar estado</label>
          <select v-model="nuevoEstado">
            <option v-for="e in estados" :key="e" :value="e">{{ e }}</option>
          </select>
        </div>
        <button @click="cambiarEstado" style="margin-bottom: 16px">Actualizar estado</button>
      </div>

      <div style="display: flex; gap: 10px">
        <button class="btn-danger" @click="cancelar">Cancelar turno</button>
        <button @click="marcarAusencia">Marcar ausencia</button>
      </div>
    </div>
    <p v-else>Cargando...</p>
  </div>
</template>

<script>
import { turnosApi } from '../services/api'

export default {
  name: 'TurnoDetalle',
  data() {
    return {
      turno: null,
      nuevoEstado: 'Pendiente',
      estados: ['Pendiente', 'Confirmado', 'Cancelado', 'Atendido', 'NoShow']
    }
  },
  async mounted() {
    try {
      const res = await turnosApi.getById(this.$route.params.id)
      this.turno = res.data
      this.nuevoEstado = this.turno.estado
    } catch {
      alert('Error al procesar la solicitud')
    }
  },
  methods: {
    formatFecha(fecha) {
      return new Date(fecha).toLocaleString('es-AR')
    },
    async cambiarEstado() {
      try {
        const res = await turnosApi.actualizarEstado(this.turno.id, { estado: this.nuevoEstado })
        this.turno = res.data
      } catch (e) {
        alert(e.response?.data?.mensaje || 'Error al actualizar el estado')
      }
    },
    async cancelar() {
      try {
        const res = await turnosApi.cancelar(this.turno.id)
        this.turno = res.data
      } catch (e) {
        alert(e.response?.data?.mensaje || 'Error al cancelar el turno')
      }
    },
    async marcarAusencia() {
      try {
        const res = await turnosApi.marcarAusencia(this.turno.id)
        this.turno = res.data
      } catch (e) {
        alert(e.response?.data?.mensaje || 'Error al marcar la ausencia')
      }
    }
  }
}
</script>

<style scoped>
.detail-row {
  display: flex;
  padding: 10px 0;
  border-bottom: 1px solid #f0f0f0;
  font-size: 14px;
}
.label {
  width: 130px;
  font-weight: 600;
  color: #666;
  flex-shrink: 0;
}
</style>
