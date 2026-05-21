import axios from 'axios'

const BASE_URL = 'http://localhost:5000'

export const turnosApi = {
  getAll:           ()          => axios.get(`${BASE_URL}/turnos`),
  getById:          (id)        => axios.get(`${BASE_URL}/turnos/${id}`),
  create:           (data)      => axios.post(`${BASE_URL}/turnos`, data),
  cancelar:         (id)        => axios.put(`${BASE_URL}/turnos/cancelar/${id}`),
  marcarAusencia:   (id)        => axios.post(`${BASE_URL}/turnos/${id}/ausencia`),
  actualizarEstado: (id, data)  => axios.put(`${BASE_URL}/turnos/${id}/estado`, data)
}

export const pacientesApi = {
  getAll:  ()          => axios.get(`${BASE_URL}/pacientes`),
  getById: (id)        => axios.get(`${BASE_URL}/pacientes/${id}`),
  create:  (data)      => axios.post(`${BASE_URL}/pacientes`, data),
  update:  (id, data)  => axios.put(`${BASE_URL}/pacientes/${id}`, data),
  delete:  (id)        => axios.delete(`${BASE_URL}/pacientes/${id}`)
}

export const medicosApi = {
  getAll: () => axios.get(`${BASE_URL}/medicos`)
}

export const sucursalesApi = {
  getAll: () => axios.get(`${BASE_URL}/sucursales`)
}
